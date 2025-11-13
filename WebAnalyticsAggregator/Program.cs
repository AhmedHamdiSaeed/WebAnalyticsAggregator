
using Application.Implementations;
using Application.Interfaces;
using Infrastructure.interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Data.SqlClient;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Add EF Core
            builder.Services.AddDbContext<AnalyticsDbContext>(options =>
            options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.MigrationsAssembly("Infrastructure") // <-- Important
                                                                      ));
            builder.Services.AddControllers();
            // 🔹 Register Repositories (Infrastructure)
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();

            // 🔹 Register Services (Application)
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IReportService, ReportService>();

            // 🔹 Register JWT Provider
            builder.Services.AddScoped<IJwtProvider, JwtProvider>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecretKey"))
                };
            });
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();
                var retries = 10;
                while (retries > 0)
                {
                    try
                    {
                        db.Database.Migrate();
                        break; // success
                    }
                    catch (SqlException)
                    {
                        retries--;
                        Console.WriteLine("Waiting for SQL Server...");
                        Thread.Sleep(5000); // wait 5 seconds
                    }
                }
            }

            // Configure the HTTP request pipeline.

            app.UseSwagger();
                app.UseSwaggerUI();
            

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

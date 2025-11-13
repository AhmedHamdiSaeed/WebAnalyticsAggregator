using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Implementations;
using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.IntegrationTests
{
    public class UserServiceIntegrationTests
    {
        private readonly AnalyticsDbContext _dbContext;
        private readonly IUserService _userService;

        public UserServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<AnalyticsDbContext>()
                .UseSqlite("DataSource=:memory:") // in-memory SQLite
                .Options;

            _dbContext = new AnalyticsDbContext(options);
            _dbContext.Database.OpenConnection();
            _dbContext.Database.EnsureCreated();

            var userRepo = new UserRepository(_dbContext); // real repository
            var jwtProvider = new FakeJwtProvider(); // simple fake JWT for testing
            _userService = new UserService(userRepo, jwtProvider);
        }

        [Fact]
        public async Task RegisterAndLogin_EndToEnd()
        {
            // Register
            var register = await _userService.RegisterAsync("John", "john@test.com", "Pass123");
            Assert.True(register.IsSuccess);
            Assert.Equal("USER_REGISTERED", register.Code);

            // Login success
            var login = await _userService.LoginAsync("john@test.com", "Pass123");
            Assert.True(login.IsSuccess);
            Assert.Equal("LOGIN_SUCCESS", login.Code);

            // Login failure
            var failLogin = await _userService.LoginAsync("john@test.com", "WrongPass");
            Assert.False(failLogin.IsSuccess);
            Assert.Equal("INVALID_CREDENTIALS", failLogin.Code);
        }
    }

    // Fake JWT for integration testing
    public class FakeJwtProvider : IJwtProvider
    {
        public string GenerateToken(User user) => "fake-jwt";
    }
}

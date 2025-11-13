# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files for restore
COPY DTOs/DTOs.csproj DTOs/
COPY Application/Application.csproj Application/
COPY Producer/Producer.csproj  Producer/


# Restore dependencies
RUN dotnet restore "Producer/Producer.csproj"

# Copy everything else
COPY . .

# Set working directory to Producer
WORKDIR "/src/Producer"

# Publish the app
RUN dotnet publish "Producer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------- Runtime Stage ----------
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Producer.dll"]
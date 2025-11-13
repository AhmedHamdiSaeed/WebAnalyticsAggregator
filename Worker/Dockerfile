# Base runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore
COPY ["Consumer.csproj", "./"]
RUN dotnet restore "./Consumer.csproj"

# Copy all files and build
COPY . .
RUN dotnet build "./Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
RUN dotnet publish "./Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Consumer.dll"]
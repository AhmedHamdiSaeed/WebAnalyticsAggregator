FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY DTOs/DTOs.csproj DTOs/

COPY WebAnalyticsAggregator/WebAnalyticsAggregator.csproj WebAnalyticsAggregator/
RUN dotnet restore "WebAnalyticsAggregator/WebAnalyticsAggregator.csproj"

COPY . .
WORKDIR /src/WebAnalyticsAggregator
RUN dotnet publish -c Release -o /app/out /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "WebAnalyticsAggregator.dll"]
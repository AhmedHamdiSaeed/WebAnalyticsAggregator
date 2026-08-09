FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DTOs/DTOs.csproj DTOs/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Worker/Consumer.csproj Worker/

RUN dotnet restore "Worker/Consumer.csproj"

COPY . .
WORKDIR "/src/Worker"
RUN dotnet publish "Consumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Consumer.dll"]
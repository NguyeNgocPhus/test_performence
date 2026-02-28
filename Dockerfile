FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY ["test_peformance/test_peformance.csproj", "test_peformance/"]
COPY ["IntegrationEventLogEF/IntegrationEventLogEF.csproj", "IntegrationEventLogEF/"]
COPY ["EventBus/EventBus.csproj", "EventBus/"]
COPY ["EventBusRabbitMQ/EventBusRabbitMQ.csproj", "EventBusRabbitMQ/"]

RUN dotnet restore "test_peformance/test_peformance.csproj"

COPY . .
WORKDIR /src/test_peformance
RUN dotnet publish "test_peformance.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine

WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000 \
    ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "test_peformance.dll"]
# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the solution
dotnet build test_peformance.sln

# Run the main web API
dotnet run --project test_peformance/test_peformance.csproj

# Run with a specific environment
dotnet run --project test_peformance/test_peformance.csproj --environment Development

# Run tests
dotnet test TestProject1/TestProject1.csproj

# Add EF Core migration
dotnet ef migrations add "<MigrationName>" --project test_peformance --context ApplicationDbContext --startup-project test_peformance --output-dir Migrations

# Apply EF Core migrations
dotnet ef database update --project test_peformance --startup-project test_peformance --context ApplicationDbContext
```

## Architecture Overview

This is a .NET 9 ASP.NET Core Web API solution used as a performance/architecture testing playground. It integrates multiple infrastructure components.

### Solution Projects

- **test_peformance** — Main web API (the primary project)
- **EventBus** — Abstraction layer for event-driven messaging (`IEventBus`, `IntegrationEvent`, subscription management)
- **EventBusRabbitMQ** — RabbitMQ implementation of `IEventBus` using `IRabbitMqPersistentConnection`
- **IntegrationEventLogEF** — EF Core-backed outbox log for integration events (`IntegrationEventLogEntry`, `IntegrationEventLogContext`)
- **TestProject1** — Unit test project
- **CronJob** — Background/scheduled job project

### Key Patterns

**Repository + Unit of Work**: `IRepositoryBase<TEntity, TKey>` / `IUnitOfWork` abstractions with two concrete repository implementations (`RepositoryBase`, `RepositoryBase2`). Both are registered in DI — the last registration (`RepositoryBase<,>`) wins.

**Event Bus**: Integration events extend `IntegrationEvent` from the `EventBus` project. Events are published/subscribed via `IEventBus` (backed by RabbitMQ). Event handlers implement `IIntegrationEventHandler<T>` and must be registered as scoped services.

**Orleans Grains**: Grain interfaces live in `Abstractions/` (e.g., `IChat`, `IHelloGrain`, `IProductGrain`). Grain implementations are in `Grains/`. Orleans is configured with Redis clustering and persistence.

**Kafka Consumer**: `KafkaConsumerBackgroundService` is a hosted service driven by `KafkaConsumerOptions` (bound from `appsettings.json` under `"Kafka"`). Toggle via `Kafka:Enabled`.

**SignalR**: A `ChatHub` is mapped at `/streaming-hub` with JWT bearer auth (token read from `access_token` query param).

**OpenTelemetry**: Traces and metrics exported via OTLP to `OTEL_EXPORTER_OTLP_ENDPOINT` (default `http://localhost:4317`).

**Serilog**: Configured in `Program.cs` to write to console and rolling file (`logs/log-<date>.txt`), enriched with `RequestId` (TraceId).

### Data

- **SQL Server** — Primary database via EF Core (`ApplicationDbContext`). Connection string key: `ConnectionStrings:ConnectionStrings`.
- **Redis** — Caching and Orleans clustering. Connection string key: `ConnectionStrings:Redis`.
- **PostgreSQL** — `AppDbOption` section in config; driver package present but not actively used in `ApplicationDbContext`.

### Configuration Keys (appsettings.Development.json)

| Key | Purpose |
|-----|---------|
| `ConnectionStrings:ConnectionStrings` | SQL Server connection |
| `ConnectionStrings:Redis` | Redis connection |
| `EventBusConnection` | RabbitMQ hostname |
| `EventBusUserName` / `EventBusPassword` | RabbitMQ credentials |
| `SubscriptionClientName` | RabbitMQ queue name |
| `EventBusRetryCount` | Retry count for event bus |
| `Kafka:*` | Kafka consumer settings |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | OpenTelemetry collector endpoint |

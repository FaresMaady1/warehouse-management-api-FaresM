# Warehouse Management API

An ASP.NET Core Web API for managing warehouse inventory — products, suppliers, and stock
movements. Starting from a simple in-memory CRUD API, moving through domain-driven layering and real database
persistence, and finishing with the hardening, observability, and performance features
expected of a production-ready service.

## Tech Used

- ASP.NET Core Web API (.NET 8)
- EF Core + PostgreSQL (Docker)
- MediatR (CQRS-style commands/queries)
- AutoMapper
- Redis (distributed caching)
- Serilog (structured logging)
- Hangfire (recurring background jobs)
- Health Checks + Health Checks UI
- xUnit

## Architecture

Four layers, dependencies pointing inward:

```
Warehouse.Domain          zero external dependencies — entities, business rules, repository interfaces
Warehouse.Application  -> commands/queries + handlers (MediatR), one use case each
Warehouse.Infrastructure  -> EF Core persistence, Redis caching, repository implementations
WebApi                    -> HTTP entry point (controllers, DI/startup, request contracts)
```

> `WebApi` fills the role of a `Warehouse.Presentation` layer — it was kept as `WebApi` rather
> than renamed, a decision made and documented in Session 03 to avoid re-touching an
> already-large refactor.

## Sessions

### Session 02 — First API
- In-memory CRUD API for products and suppliers using the Controller-Service pattern
- Git hygiene: root-level `.gitignore`, untracked `bin/`/`obj/`
- Model validation via data annotation attributes on request DTOs

### Session 03 — DDD Architecture Refactor
- Refactored into the four-layer architecture above
- Every use case now runs through a MediatR command/query + handler instead of a direct
  service call
- No endpoints added, removed, or behaviorally changed — same routes, new internal structure

### Session 04 — Databases, EF Core & LINQ
- Connected the API to PostgreSQL via EF Core; both DB First and Code First approaches were
  explored, with Code First merged into the main app
- Real `WarehouseDbContext`, repository `SaveChangesAsync()`, EF Core migrations with seed
  data via `HasData`
- AutoMapper `ProductViewModel` / `SupplierViewModel` replacing raw response records in
  controller output
- Bonus: OData query endpoints

### Session 05 — Hardening the API
- Centralized error handling: `ApiErrorResponse`, `DomainException` / `NotFoundException`,
  `ExceptionStatusMapper`
- Request validation on `CreateProductRequest` and `CreateStockAdjustmentRequest`
- New middleware: exception handling, correlation ID, request timing
- Repositories and handlers converted to real async EF Core calls with `CancellationToken`
  support throughout
- New `/api/inventory/dashboard` aggregate endpoint

### Session 06 — Observability & Performance
- Localization (English / French / Arabic) with Swagger culture control via `Accept-Language`
- Structured logging with Serilog, written to console and rolling daily log files
- Redis-backed distributed caching on all read endpoints, invalidated on every write
- Health Checks (Postgres + Redis) exposed at `/health`, visualized via Health Checks UI at
  `/health-ui`
- Recurring Hangfire background job that checks for expired / soon-to-expire products

**Bonus challenges completed:**
1. Background Job Improvement — auto-archives products expired more than 7 days
2. Slow Request Logging — logs any request over 500ms with endpoint, method, status, duration
3. Cache Statistics Endpoint — `GET /api/cache/statistics` (keys, hit/miss counts, last refresh)
4. Retry Health Check — Redis health check retries up to 3 times before reporting unhealthy
5. Self-identified: diagnosed and resolved a Redis connection failure caused by a host port
   mapping conflict (see PR description for details)

## Seeded Reference IDs

Data resets on every restart but always seeds with these fixed GUIDs, so you can hit `GET` /
`POST` / `DELETE` endpoints straight away without a `POST` first just to get an id.

**Suppliers**

| Id | Name |
|---|---|
| `72338e71-fe24-44d6-a6ae-5396bd2ce8bb` | TechSupply Co. |
| `6964d19b-0fa7-4cb0-ab62-dbdd1fcd43c5` | Green Valley Farms |
| `8ac4d89a-bef3-47c8-883f-ecade15fd80f` | Golden Harvest Ltd. |
| `d9bee142-bbf4-4ed6-a398-f6da24f7bfc3` | Mediterra Imports |
| `b54f534a-3d73-46bd-95a4-edf561d36ab2` | Sunny Fields Beverages |

**Products**

| Id | Name | SKU |
|---|---|---|
| `3f2504e0-4f89-11d3-9a0c-0305e82c3301` | Wireless Mouse | ELEC-001 |
| `2b60a60d-e313-4454-9d9f-a1739f58aa87` | Mechanical Keyboard | ELEC-002 |
| `86451dd2-da87-4362-a186-5e04aa125afb` | Whole Milk | DAIRY-001 |
| `c376238a-2723-4ea1-bb48-9298a27a58f2` | Cheddar Cheese | DAIRY-002 |
| `d21df4a5-cd5f-4345-9bcc-1e018bef411f` | Basmati Rice | GRAIN-001 |
| `acdac899-bbf3-4563-9c15-d25064255fbc` | Olive Oil | OIL-001 |
| `04d8c4e0-0a74-4172-9805-a02d6aa202d2` | Bluetooth Speaker | ELEC-003 |
| `59259ca4-3749-44be-8455-ce4392577fc1` | Orange Juice | BEV-001 |
| `f21a8cb4-5565-474e-8ceb-dd1a123ed6c0` | Pasta | GRAIN-002 |
| `0d18b21e-20cf-4b83-9424-9c2932ad5787` | Green Tea | BEV-002 |

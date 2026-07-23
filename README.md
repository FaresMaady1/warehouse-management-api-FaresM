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

### Session 07 — Firebase Authentication, Authorization & MinIO Storage

**Firebase Authentication**
- Every request needs a valid Firebase ID token in the `Authorization: Bearer` header
- The API checks the token's signature, issuer, audience, and expiry before letting the request through 

**Authorization **
- All endpoints require a signed-in user by default so i had to add (RequireAuthenticatedUser) in a filter in program.cs
- So any signed in user can use any enpoint exept where the endpoint has a decorator [Authorise (Roles =admin)]
- So the authorization is done internaly and not managed by firebase 

**Object Storage (Minio)**
- `IFileStorageService` is an interface for uploading, downloading, and deleting files
- `MinioFileStorageService` implements it using the Minio SDK

**Product Images moved to be stored in Minio and DB**
- Before the image was stored in WWWroot
- Now uploading a new image replaces the old one, both in Minio and in the database
- Only the file name, type, and Minio key are stored in the database not the file itself

**Supplier Documents**
- Same idea as product images, but a supplier can have more than one document

**Role Bootstrap Endpoint**
- Because the authorisation is done localy, on program startup a user should be assigned a role using this endpoint
- So it is the only endpoint that can be accesed with no jwt token, i generated a random key to use in this case
- I think this is better for testing purposes to be able to change the role of a user on the go to be able to test different senarios rather than hard coding each user and its roles

**Swagger Authorize Button**
- Adds an "Authorize" button in Swagger so a token can be pasted once and reused for every request
- But i needed to get the token throw a cmd login using rest and the api of my firebase

**Note that the screenshots of this lab are at the end of the readme**

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

## Latest App Features Screenshots
## Session 6
### Localization

<img width="1480" height="796" alt="image" src="https://github.com/user-attachments/assets/fd550e04-8709-4c47-8331-2f0a1ebf24a8" />
<img width="1335" height="758" alt="image" src="https://github.com/user-attachments/assets/36435948-443c-46eb-9002-7f5ddde06b0d" />

### Structured Logging in log file content

<img width="1255" height="301" alt="image" src="https://github.com/user-attachments/assets/0976d08f-3f5c-4e04-a95a-c34dfe6ae39e" />

### Cache Statistics 

<img width="1760" height="712" alt="image" src="https://github.com/user-attachments/assets/771d9f14-8c68-4633-95d6-693ae36fcf18" />

2 misses contribute in 2 new cache entries
<img width="431" height="185" alt="image" src="https://github.com/user-attachments/assets/cd332ff5-a779-46e5-bdce-d1f007a68ef0" />


### Health Checks

Normal
<img width="1904" height="403" alt="image" src="https://github.com/user-attachments/assets/2a2efcf9-8ab8-433e-9151-16ed3f85025b" />

Redis down with retry
<img width="1894" height="392" alt="image" src="https://github.com/user-attachments/assets/7913db9c-d5e8-49e3-8df8-f4cb0fac8699" />

### Hangfire (check expiration date triggered manually) with auto archive

<img width="1854" height="916" alt="image" src="https://github.com/user-attachments/assets/26c800f1-79ac-4b07-8bbb-e6c303679b04" />

Auto Archived the product after the hangfire trigger
<img width="944" height="422" alt="image" src="https://github.com/user-attachments/assets/f1e995ab-4b4b-4769-976f-f2d4c0fc432f" />

### Slow request log warnning (threshold = 500 ms)

<img width="830" height="118" alt="image" src="https://github.com/user-attachments/assets/694fa663-63f5-49ba-9898-171df2f4ee9b" />



## Session 7

### Using the normal user token (normal get and forbidden post)
<img width="1783" height="711" alt="image" src="https://github.com/user-attachments/assets/cb83a181-0706-4180-9682-0a6b7d521d29" />
<img width="1754" height="692" alt="image" src="https://github.com/user-attachments/assets/61107f25-22d5-49c0-9ee1-7b7b8d3e5f1f" />

### Trying with no token (401)
<img width="1774" height="532" alt="image" src="https://github.com/user-attachments/assets/9cbb3b30-0e7f-4986-ab33-f9cd38c3aa39" />

### Trying with expired token (401)
<img width="1781" height="548" alt="image" src="https://github.com/user-attachments/assets/8db9ef78-cf8c-426e-87c5-5ec6fef6ed2f" />

### Minio bucket and tested docs and images
<img width="1878" height="385" alt="image" src="https://github.com/user-attachments/assets/ffce9e15-3286-4676-a351-39bee70c4ccf" />
<img width="1535" height="276" alt="image" src="https://github.com/user-attachments/assets/a1e8d47a-517c-496d-af73-c9b9e3a66ba3" />

### Firebase test users
<img width="1685" height="611" alt="image" src="https://github.com/user-attachments/assets/39c1d732-451e-4bd9-a4bc-b873220f46bd" />






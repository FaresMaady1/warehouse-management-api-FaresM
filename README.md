# Warehouse Management API

ASP.NET Core Web API. Manages warehouse products and suppliers, built incrementally session by session.

---

## Session 02 — First API

In-memory CRUD API for products and suppliers, all logic living in controllers/services.

### Product endpoints (`api/products`)

- GET `/` — all products (sorted newest first, `?onlyAvailable=true` to filter)
- GET `/{id}` — one product
- GET `/search?name=&supplier=` — search by name/supplier
- POST `/` — create product
- POST `/{id}/quantity` — update stock
- POST `/{id}/price` — update price
- POST `/{id}/image` — upload product image (jpg/png, max 2MB)
- DELETE `/{id}` — archive product (soft delete)
- GET `/server-time` — server time based on `Accept-Language` header
- POST `/{id}/assign-supplier/{supplierId}` — link a supplier to a product

### Supplier endpoints (`api/suppliers`)

- GET `/` — all suppliers
- GET `/{id}` — one supplier
- POST `/` — create supplier
- DELETE `/{id}` — deactivate supplier

### Notes

- Data is seeded on startup (10 products, 5 suppliers) and resets when the app restarts.
- Uploaded images go to `wwwroot/uploads`.

---

## Session 03 — DDD Architecture Refactor

Same API, same behavior. Refactored from a controller+service structure into a layered
architecture, with every use case now handled through MediatR instead of a direct service call.

### Architecture explanation

The project is now split into four layers, each with one job:

```
Warehouse.Domain          <- zero external dependencies. Entities + business rules + repository interfaces.
Warehouse.Application     -> references Domain only. Use cases as MediatR commands/queries + handlers.
Warehouse.Infrastructure  -> references Domain only. In-memory data store + repository implementations.
Warehouse.Presentation    -> references Application + Infrastructure. HTTP entry point.
```

**Rule:** dependencies point inward. Domain never references Application, Infrastructure, or
the web project — everything else depends on Domain, not the other way around.

> **Naming note:** `Warehouse.Presentation` above is the existing **`WebApi`** project, not a
> newly created one. I chose not to rename the `WebApi` folder/project so I wouldn't complicate
> an already-large refactor (renaming a project mid-solution touches the `.sln`, the `.csproj`,
> `launchSettings.json`, and every reference to it). By the time I'd finished the other three
> layers and realized the naming was inconsistent with the rest (`Warehouse.Domain`,
> `Warehouse.Application`, `Warehouse.Infrastructure` vs. plain `WebApi`), renaming it would
> have meant re-touching files I'd already verified working — so I left it as `WebApi` and I'm
> flagging the inconsistency here instead. Happy to rename it in a follow-up session if that's
> preferred.

### Layer responsibilities

| Layer | Responsibility | Depends on |
|---|---|---|
| `Warehouse.Domain` | `Product` / `Supplier` entities, the business rules they enforce (price > 0, quantity >= 0, no updates on archived products, no assigning inactive suppliers), `IProductRepository` / `ISupplierRepository` interfaces, `DomainException` | nothing |
| `Warehouse.Application` | One MediatR command or query per use case, one handler per command/query, organized under `Commands/`, `Queries/`, `Handlers/`, each split by entity (`Products/`, `Suppliers/`) | `Warehouse.Domain` |
| `Warehouse.Infrastructure` | `WarehouseDbContext` (in-memory store, seeded on startup — same role as the old `FakeWarehouseStore`/`FakeSupplierStore`, ready to be swapped for EF Core), `ProductRepository`, `SupplierRepository` | `Warehouse.Domain` |
| `WebApi` (= `Warehouse.Presentation`) | `ProductsController` / `SuppliersController` — thin, just call `_mediator.Send(...)` and translate the result into an HTTP response, `Program.cs` for DI/startup, `Contracts/` for request DTOs | `Warehouse.Application`, `Warehouse.Infrastructure` |

### List of refactored endpoints

No endpoints were added or removed this session — every endpoint below kept its route, verb,
and response shape, but now runs through a MediatR command/query instead of a service call.

| Endpoint | MediatR request |
|---|---|
| GET `/api/products` | `ListProductsQuery` |
| GET `/api/products/{id}` | `GetProductByIdQuery` |
| GET `/api/products/search` | `SearchProductsQuery` |
| POST `/api/products` | `CreateProductCommand` |
| POST `/api/products/{id}/quantity` | `UpdateProductQuantityCommand` |
| POST `/api/products/{id}/price` | `UpdateProductPriceCommand` |
| POST `/api/products/{id}/image` | `UploadProductImageCommand` |
| DELETE `/api/products/{id}` | `ArchiveProductCommand` |
| POST `/api/products/{id}/assign-supplier/{supplierId}` | `AssignSupplierToProductCommand` |
| GET `/api/products/server-time` | *(no use case — pure HTTP/culture logic)* |
| GET `/api/suppliers` | `ListSuppliersQuery` |
| GET `/api/suppliers/{id}` | `GetSupplierByIdQuery` |
| POST `/api/suppliers` | `CreateSupplierCommand` |
| DELETE `/api/suppliers/{id}` | `DeactivateSupplierCommand` |


### Seeded IDs — quick reference for testing

Data resets on every restart but always seeds with these fixed GUIDs, so you can hit `GET` /
`POST` / `DELETE` endpoints straight away without doing a `POST` first just to get an id.

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

Example: `GET /api/products/3f2504e0-4f89-11d3-9a0c-0305e82c3301` returns the Wireless Mouse
right away — no need to create a product first just to get an id to test with.


| Test | Expected result |
|---|---|
| `ProductTests.Create_Throws_When_Price_Is_Zero_Or_Negative` | Pass |
| `ProductTests.Create_Throws_When_Quantity_Is_Negative` | Pass |
| `ProductTests.UpdatePrice_Throws_When_Product_Is_Archived` | Pass |
| `ProductTests.AssignSupplier_Throws_When_Supplier_Is_Inactive` | Pass |
| `GetProductByIdHandlerTests.Handle_Returns_Null_When_Product_Missing` | Pass |
| `GetProductByIdHandlerTests.Handle_Returns_Product_When_Found` | Pass |


### Screenshots of Swagger

*All previous fetures still work the same*
<img width="1830" height="764" alt="image" src="https://github.com/user-attachments/assets/6c2da15c-a336-4b21-bbb3-3b2992e73d35" />
<img width="1810" height="341" alt="image" src="https://github.com/user-attachments/assets/fe4fd078-e0f8-4b5b-b507-13edcfff6893" />
Deleting a product
<img width="1795" height="661" alt="image" src="https://github.com/user-attachments/assets/07e98ed2-5f3e-4b91-b299-9b9210f93799" />
Successfuly archived 
<img width="1779" height="767" alt="image" src="https://github.com/user-attachments/assets/765886d4-7831-4332-9ac5-7535ac6045b1" />
Updating quantity to the deleted product
<img width="1776" height="735" alt="image" src="https://github.com/user-attachments/assets/331c338a-0ab3-4704-baa8-fc657a0cb390" />

### Unit tests

<img width="835" height="308" alt="image" src="https://github.com/user-attachments/assets/61b680da-3092-4069-bc33-a4ef4913974c" />

### Notes

- DTO validation attributes are still pending from Session 02 — left for direct discussion,
  same as before.
- `StockMovement` and `WarehouseItem` are modeled per the lab requirement but not yet attached
  to a repository or endpoint.
- The Presentation layer is the `WebApi` project itself, not a renamed/new one — see the naming
  note under Architecture explanation above.

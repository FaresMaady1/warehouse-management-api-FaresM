# Warehouse Management API

ASP.NET Core Web API (Session 02 lab). Manages warehouse products (and suppliers) in memory, no database.


## Product endpoints (`api/products`)

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

## Supplier endpoints (`api/suppliers`)

- GET `/` — all suppliers
- GET `/{id}` — one supplier
- POST `/` — create supplier
- DELETE `/{id}` — deactivate supplier

## Notes

- Data is seeded on startup (10 products, 5 suppliers) and resets when the app restarts.
- Uploaded images go to `wwwroot/uploads`.



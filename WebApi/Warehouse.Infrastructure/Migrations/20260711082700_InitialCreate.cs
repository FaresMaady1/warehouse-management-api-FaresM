using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Warehouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    ContactEmail = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SKU = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    QuantityInStock = table.Column<int>(type: "integer", nullable: false),
                    SupplierName = table.Column<string>(type: "text", nullable: false),
                    SupplierId = table.Column<string>(type: "text", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "ContactEmail", "Country", "IsActive", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { "6964d19b-0fa7-4cb0-ab62-dbdd1fcd43c5", "info@greenvalley.com", "Lebanon", true, "Green Valley Farms", "+961-1-234567" },
                    { "72338e71-fe24-44d6-a6ae-5396bd2ce8bb", "contact@techsupply.com", "USA", true, "TechSupply Co.", "+1-555-0101" },
                    { "8ac4d89a-bef3-47c8-883f-ecade15fd80f", "sales@goldenharvest.com", "India", true, "Golden Harvest Ltd.", "+91-22-1234567" },
                    { "b54f534a-3d73-46bd-95a4-edf561d36ab2", "contact@sunnyfields.com", "Spain", true, "Sunny Fields Beverages", "+34-91-1234567" },
                    { "d9bee142-bbf4-4ed6-a398-f6da24f7bfc3", "hello@mediterra.com", "Greece", true, "Mediterra Imports", "+30-21-0123456" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "Description", "ExpiryDate", "IsArchived", "LastUpdatedAt", "Name", "Price", "QuantityInStock", "SKU", "SupplierId", "SupplierName" },
                values: new object[,]
                {
                    { "04d8c4e0-0a74-4172-9805-a02d6aa202d2", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Portable Bluetooth speaker with 10h battery", new DateTime(2031, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bluetooth Speaker", 39.99m, 60, "ELEC-003", "72338e71-fe24-44d6-a6ae-5396bd2ce8bb", "TechSupply Co." },
                    { "0d18b21e-20cf-4b83-9424-9c2932ad5787", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Organic green tea, box of 50 bags", new DateTime(2027, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Green Tea", 6.49m, 140, "BEV-002", "d9bee142-bbf4-4ed6-a398-f6da24f7bfc3", "Mediterra Imports" },
                    { "2b60a60d-e313-4454-9d9f-a1739f58aa87", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "RGB backlit mechanical keyboard", new DateTime(2031, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mechanical Keyboard", 59.99m, 80, "ELEC-002", "72338e71-fe24-44d6-a6ae-5396bd2ce8bb", "TechSupply Co." },
                    { "3f2504e0-4f89-11d3-9a0c-0305e82c3301", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ergonomic wireless mouse with USB receiver", new DateTime(2031, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wireless Mouse", 19.99m, 150, "ELEC-001", "72338e71-fe24-44d6-a6ae-5396bd2ce8bb", "TechSupply Co." },
                    { "59259ca4-3749-44be-8455-ce4392577fc1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "100% pure orange juice, 1L", new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Orange Juice", 4.29m, 175, "BEV-001", "b54f534a-3d73-46bd-95a4-edf561d36ab2", "Sunny Fields Beverages" },
                    { "86451dd2-da87-4362-a186-5e04aa125afb", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "1 gallon whole milk", new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Whole Milk", 3.49m, 200, "DAIRY-001", "6964d19b-0fa7-4cb0-ab62-dbdd1fcd43c5", "Green Valley Farms" },
                    { "acdac899-bbf3-4563-9c15-d25064255fbc", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Extra virgin olive oil, 1L", new DateTime(2028, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Olive Oil", 9.99m, 90, "OIL-001", "d9bee142-bbf4-4ed6-a398-f6da24f7bfc3", "Mediterra Imports" },
                    { "c376238a-2723-4ea1-bb48-9298a27a58f2", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sharp cheddar cheese block, 500g", new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cheddar Cheese", 5.99m, 120, "DAIRY-002", "6964d19b-0fa7-4cb0-ab62-dbdd1fcd43c5", "Green Valley Farms" },
                    { "d21df4a5-cd5f-4345-9bcc-1e018bef411f", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "5kg bag of premium basmati rice", new DateTime(2027, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Basmati Rice", 12.99m, 300, "GRAIN-001", "8ac4d89a-bef3-47c8-883f-ecade15fd80f", "Golden Harvest Ltd." },
                    { "f21a8cb4-5565-474e-8ceb-dd1a123ed6c0", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Durum wheat pasta, 1kg", new DateTime(2027, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pasta", 2.49m, 250, "GRAIN-002", "8ac4d89a-bef3-47c8-883f-ecade15fd80f", "Golden Harvest Ltd." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}

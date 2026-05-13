using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProductionSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "equipment_types",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment_types", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "production_operations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_production_operations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Dimensions = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    DeliveryDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Login = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Password = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    FullName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Photo = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Login);
                });

            migrationBuilder.CreateTable(
                name: "warehouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LastName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FirstMiddleName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    HomeAddress = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Education = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Qualification = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "equipment",
                columns: table => new
                {
                    Marking = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EquipmentTypeName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Characteristics = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment", x => x.Marking);
                    table.ForeignKey(
                        name: "FK_equipment_equipment_types_EquipmentTypeName",
                        column: x => x.EquipmentTypeName,
                        principalTable: "equipment_types",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_assembly_specs",
                columns: table => new
                {
                    ParentProductName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ChildProductName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_assembly_specs", x => new { x.ParentProductName, x.ChildProductName });
                    table.ForeignKey(
                        name: "FK_product_assembly_specs_products_ChildProductName",
                        column: x => x.ChildProductName,
                        principalTable: "products",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_product_assembly_specs_products_ParentProductName",
                        column: x => x.ParentProductName,
                        principalTable: "products",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_operation_specs",
                columns: table => new
                {
                    ProductName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    OperationId = table.Column<int>(type: "integer", nullable: false),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false),
                    EquipmentTypeName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_operation_specs", x => new { x.ProductName, x.OperationId, x.SequenceNumber });
                    table.ForeignKey(
                        name: "FK_product_operation_specs_equipment_types_EquipmentTypeName",
                        column: x => x.EquipmentTypeName,
                        principalTable: "equipment_types",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_product_operation_specs_production_operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "production_operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_product_operation_specs_products_ProductName",
                        column: x => x.ProductName,
                        principalTable: "products",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customer_orders",
                columns: table => new
                {
                    Number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    OrderName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    CustomerLogin = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ManagerLogin = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EstimatedCost = table.Column<decimal>(type: "numeric", nullable: true),
                    PlannedCompletionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CustomerDrawings = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_orders", x => x.Number);
                    table.ForeignKey(
                        name: "FK_customer_orders_products_ProductName",
                        column: x => x.ProductName,
                        principalTable: "products",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_orders_users_CustomerLogin",
                        column: x => x.CustomerLogin,
                        principalTable: "users",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_orders_users_ManagerLogin",
                        column: x => x.ManagerLogin,
                        principalTable: "users",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "components",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Article = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Unit = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    Image = table.Column<byte[]>(type: "bytea", nullable: true),
                    ComponentType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric", nullable: false),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_components", x => x.Id);
                    table.ForeignKey(
                        name: "FK_components_suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "suppliers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_components_warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Article = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Unit = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: true),
                    Image = table.Column<byte[]>(type: "bytea", nullable: true),
                    MaterialType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Gost = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Length = table.Column<decimal>(type: "numeric", nullable: true),
                    Characteristics = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    WarehouseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_materials_suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "suppliers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_materials_warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worker_operations",
                columns: table => new
                {
                    WorkerId = table.Column<int>(type: "integer", nullable: false),
                    OperationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worker_operations", x => new { x.WorkerId, x.OperationId });
                    table.ForeignKey(
                        name: "FK_worker_operations_production_operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "production_operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_worker_operations_workers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_component_specs",
                columns: table => new
                {
                    ProductName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ComponentId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_component_specs", x => new { x.ProductName, x.ComponentId });
                    table.ForeignKey(
                        name: "FK_product_component_specs_components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_component_specs_products_ProductName",
                        column: x => x.ProductName,
                        principalTable: "products",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_material_specs",
                columns: table => new
                {
                    ProductName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    MaterialId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_material_specs", x => new { x.ProductName, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_product_material_specs_materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_material_specs_products_ProductName",
                        column: x => x.ProductName,
                        principalTable: "products",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_components_Article_WarehouseId",
                table: "components",
                columns: new[] { "Article", "WarehouseId" });

            migrationBuilder.CreateIndex(
                name: "IX_components_SupplierId",
                table: "components",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_components_WarehouseId",
                table: "components",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_customer_orders_CustomerLogin",
                table: "customer_orders",
                column: "CustomerLogin");

            migrationBuilder.CreateIndex(
                name: "IX_customer_orders_ManagerLogin",
                table: "customer_orders",
                column: "ManagerLogin");

            migrationBuilder.CreateIndex(
                name: "IX_customer_orders_ProductName",
                table: "customer_orders",
                column: "ProductName");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_EquipmentTypeName",
                table: "equipment",
                column: "EquipmentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_materials_Article_WarehouseId",
                table: "materials",
                columns: new[] { "Article", "WarehouseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_materials_SupplierId",
                table: "materials",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_materials_WarehouseId",
                table: "materials",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_product_assembly_specs_ChildProductName",
                table: "product_assembly_specs",
                column: "ChildProductName");

            migrationBuilder.CreateIndex(
                name: "IX_product_component_specs_ComponentId",
                table: "product_component_specs",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_product_material_specs_MaterialId",
                table: "product_material_specs",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_product_operation_specs_EquipmentTypeName",
                table: "product_operation_specs",
                column: "EquipmentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_product_operation_specs_OperationId",
                table: "product_operation_specs",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_production_operations_Name",
                table: "production_operations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_Name",
                table: "suppliers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_worker_operations_OperationId",
                table: "worker_operations",
                column: "OperationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customer_orders");

            migrationBuilder.DropTable(
                name: "equipment");

            migrationBuilder.DropTable(
                name: "product_assembly_specs");

            migrationBuilder.DropTable(
                name: "product_component_specs");

            migrationBuilder.DropTable(
                name: "product_material_specs");

            migrationBuilder.DropTable(
                name: "product_operation_specs");

            migrationBuilder.DropTable(
                name: "worker_operations");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "components");

            migrationBuilder.DropTable(
                name: "materials");

            migrationBuilder.DropTable(
                name: "equipment_types");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "production_operations");

            migrationBuilder.DropTable(
                name: "workers");

            migrationBuilder.DropTable(
                name: "suppliers");

            migrationBuilder.DropTable(
                name: "warehouses");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProductionSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class Session2_OrdersWorkshops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductDescription",
                table: "customer_orders",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "customer_orders",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "customer_orders",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "equipment_failures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentMarking = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    RegisteredByLogin = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment_failures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_equipment_failures_equipment_EquipmentMarking",
                        column: x => x.EquipmentMarking,
                        principalTable: "equipment",
                        principalColumn: "Marking",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_equipment_failures_users_RegisteredByLogin",
                        column: x => x.RegisteredByLogin,
                        principalTable: "users",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_dimensions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Unit = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_dimensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_dimensions_customer_orders_OrderNumber",
                        column: x => x.OrderNumber,
                        principalTable: "customer_orders",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_quality_checks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ParameterName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Grade = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CheckedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CheckedByLogin = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_quality_checks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_quality_checks_customer_orders_OrderNumber",
                        column: x => x.OrderNumber,
                        principalTable: "customer_orders",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_status_history",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangedByLogin = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_status_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_status_history_customer_orders_OrderNumber",
                        column: x => x.OrderNumber,
                        principalTable: "customer_orders",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workshops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FloorPlanImage = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workshops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "workshop_layout_items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkshopId = table.Column<int>(type: "integer", nullable: false),
                    IconType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    X = table.Column<double>(type: "double precision", nullable: false),
                    Y = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workshop_layout_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workshop_layout_items_workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_equipment_failures_EquipmentMarking",
                table: "equipment_failures",
                column: "EquipmentMarking");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_failures_RegisteredByLogin",
                table: "equipment_failures",
                column: "RegisteredByLogin");

            migrationBuilder.CreateIndex(
                name: "IX_order_dimensions_OrderNumber",
                table: "order_dimensions",
                column: "OrderNumber");

            migrationBuilder.CreateIndex(
                name: "IX_order_quality_checks_OrderNumber",
                table: "order_quality_checks",
                column: "OrderNumber");

            migrationBuilder.CreateIndex(
                name: "IX_order_status_history_OrderNumber",
                table: "order_status_history",
                column: "OrderNumber");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_layout_items_WorkshopId",
                table: "workshop_layout_items",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_workshops_Name",
                table: "workshops",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "equipment_failures");

            migrationBuilder.DropTable(
                name: "order_dimensions");

            migrationBuilder.DropTable(
                name: "order_quality_checks");

            migrationBuilder.DropTable(
                name: "order_status_history");

            migrationBuilder.DropTable(
                name: "workshop_layout_items");

            migrationBuilder.DropTable(
                name: "workshops");

            migrationBuilder.DropColumn(
                name: "ProductDescription",
                table: "customer_orders");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "customer_orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "customer_orders");
        }
    }
}

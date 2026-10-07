using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutoService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "car_brands",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_car_brands", x => x.id);
                },
                comment: "Марки автомобилей");

            migrationBuilder.CreateTable(
                name: "car_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_car_categories", x => x.id);
                },
                comment: "Категории автомобилей");

            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.id);
                },
                comment: "Клиенты");

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.id);
                },
                comment: "Отделы автосервиса");

            migrationBuilder.CreateTable(
                name: "repair_bays",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    kind = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_repair_bays", x => x.id);
                    table.CheckConstraint("ck_repair_bays_kind", "kind in ('Подъёмник', 'Диагностический пост', 'Шиномонтажная зона', 'Кузовной участок')");
                },
                comment: "Ремонтные места");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                },
                comment: "Роли пользователей");

            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    base_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    duration_minutes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_services", x => x.id);
                    table.CheckConstraint("ck_services_duration", "duration_minutes > 0");
                    table.CheckConstraint("ck_services_price", "base_price >= 0");
                },
                comment: "Услуги");

            migrationBuilder.CreateTable(
                name: "specializations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_specializations", x => x.id);
                },
                comment: "Специализации механиков");

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.id);
                },
                comment: "Поставщики");

            migrationBuilder.CreateTable(
                name: "cars",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    car_brand_id = table.Column<int>(type: "integer", nullable: false),
                    car_category_id = table.Column<int>(type: "integer", nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    manufacture_year = table.Column<int>(type: "integer", nullable: false),
                    vin = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    license_plate = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    mileage = table.Column<int>(type: "integer", nullable: false),
                    color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cars", x => x.id);
                    table.UniqueConstraint("ak_cars_id_client_id", x => new { x.id, x.client_id });
                    table.CheckConstraint("ck_cars_mileage", "mileage >= 0");
                    table.CheckConstraint("ck_cars_vin_length", "char_length(vin) = 17");
                    table.CheckConstraint("ck_cars_year", "manufacture_year >= 1970 and manufacture_year <= 2100");
                    table.ForeignKey(
                        name: "FK_cars_car_brands_car_brand_id",
                        column: x => x.car_brand_id,
                        principalTable: "car_brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cars_car_categories_car_category_id",
                        column: x => x.car_category_id,
                        principalTable: "car_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cars_clients_client_id",
                        column: x => x.client_id,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Автомобили");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    login = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Учётные записи сотрудников");

            migrationBuilder.CreateTable(
                name: "parts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    supplier_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sku = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    purchase_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    sale_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    min_quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parts", x => x.id);
                    table.CheckConstraint("ck_parts_min_quantity", "min_quantity >= 0");
                    table.CheckConstraint("ck_parts_purchase_price", "purchase_price >= 0");
                    table.CheckConstraint("ck_parts_quantity", "quantity >= 0");
                    table.CheckConstraint("ck_parts_sale_price", "sale_price >= 0");
                    table.ForeignKey(
                        name: "FK_parts_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Запчасти");

            migrationBuilder.CreateTable(
                name: "mechanics",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    department_id = table.Column<int>(type: "integer", nullable: false),
                    specialization_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mechanics", x => x.id);
                    table.ForeignKey(
                        name: "FK_mechanics_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mechanics_specializations_specialization_id",
                        column: x => x.specialization_id,
                        principalTable: "specializations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mechanics_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Механики");

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    car_id = table.Column<int>(type: "integer", nullable: false),
                    service_id = table.Column<int>(type: "integer", nullable: false),
                    mechanic_id = table.Column<int>(type: "integer", nullable: false),
                    repair_bay_id = table.Column<int>(type: "integer", nullable: false),
                    scheduled_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.id);
                    table.CheckConstraint("ck_appointments_status", "status in ('Запланирована', 'Подтверждена', 'Отменена', 'Выполнена')");
                    table.ForeignKey(
                        name: "FK_appointments_cars_car_id_client_id",
                        columns: x => new { x.car_id, x.client_id },
                        principalTable: "cars",
                        principalColumns: new[] { "id", "client_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_clients_client_id",
                        column: x => x.client_id,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_mechanics_mechanic_id",
                        column: x => x.mechanic_id,
                        principalTable: "mechanics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_repair_bays_repair_bay_id",
                        column: x => x.repair_bay_id,
                        principalTable: "repair_bays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Записи на обслуживание");

            migrationBuilder.CreateTable(
                name: "mechanic_schedules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mechanic_id = table.Column<int>(type: "integer", nullable: false),
                    repair_bay_id = table.Column<int>(type: "integer", nullable: false),
                    starts_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ends_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mechanic_schedules", x => x.id);
                    table.CheckConstraint("ck_mechanic_schedules_period", "ends_at > starts_at");
                    table.ForeignKey(
                        name: "FK_mechanic_schedules_mechanics_mechanic_id",
                        column: x => x.mechanic_id,
                        principalTable: "mechanics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mechanic_schedules_repair_bays_repair_bay_id",
                        column: x => x.repair_bay_id,
                        principalTable: "repair_bays",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Расписание механиков");

            migrationBuilder.CreateTable(
                name: "mechanic_services",
                columns: table => new
                {
                    mechanic_id = table.Column<int>(type: "integer", nullable: false),
                    service_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mechanic_services", x => new { x.mechanic_id, x.service_id });
                    table.ForeignKey(
                        name: "FK_mechanic_services_mechanics_mechanic_id",
                        column: x => x.mechanic_id,
                        principalTable: "mechanics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mechanic_services_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Связь механиков и услуг");

            migrationBuilder.CreateTable(
                name: "work_orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    appointment_id = table.Column<int>(type: "integer", nullable: false),
                    mechanic_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    fault_description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_orders", x => x.id);
                    table.CheckConstraint("ck_work_orders_status", "status in ('Создан', 'Диагностика', 'В ремонте', 'Завершён', 'Отменён')");
                    table.ForeignKey(
                        name: "FK_work_orders_appointments_appointment_id",
                        column: x => x.appointment_id,
                        principalTable: "appointments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_orders_mechanics_mechanic_id",
                        column: x => x.mechanic_id,
                        principalTable: "mechanics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Заказ-наряды");

            migrationBuilder.CreateTable(
                name: "inspection_results",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    work_order_id = table.Column<int>(type: "integer", nullable: false),
                    engine_state = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    brake_state = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    suspension_state = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    electrical_state = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    recommendations = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inspection_results", x => x.id);
                    table.ForeignKey(
                        name: "FK_inspection_results_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Результаты технического осмотра");

            migrationBuilder.CreateTable(
                name: "invoices",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    work_order_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    amount_before_discount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    discount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    tax = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    total = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoices", x => x.id);
                    table.CheckConstraint("ck_invoices_amount", "amount_before_discount >= 0");
                    table.CheckConstraint("ck_invoices_discount", "discount >= 0 and discount <= amount_before_discount");
                    table.CheckConstraint("ck_invoices_status", "status in ('Выставлен', 'Частично оплачен', 'Оплачен')");
                    table.CheckConstraint("ck_invoices_tax", "tax >= 0");
                    table.CheckConstraint("ck_invoices_tax_formula", "tax = round((amount_before_discount - discount) * tax_rate / 100, 2)");
                    table.CheckConstraint("ck_invoices_tax_rate", "tax_rate >= 0 and tax_rate <= 100");
                    table.CheckConstraint("ck_invoices_total", "total = amount_before_discount - discount + tax");
                    table.ForeignKey(
                        name: "FK_invoices_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Счета");

            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    work_order_id = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.id);
                    table.CheckConstraint("ck_reviews_rating", "rating >= 1 and rating <= 5");
                    table.ForeignKey(
                        name: "FK_reviews_clients_client_id",
                        column: x => x.client_id,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reviews_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Отзывы клиентов");

            migrationBuilder.CreateTable(
                name: "work_order_parts",
                columns: table => new
                {
                    work_order_id = table.Column<int>(type: "integer", nullable: false),
                    part_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_order_parts", x => new { x.work_order_id, x.part_id });
                    table.CheckConstraint("ck_work_order_parts_price", "unit_price >= 0");
                    table.CheckConstraint("ck_work_order_parts_quantity", "quantity > 0");
                    table.ForeignKey(
                        name: "FK_work_order_parts_parts_part_id",
                        column: x => x.part_id,
                        principalTable: "parts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_order_parts_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Запчасти в заказ-нарядах");

            migrationBuilder.CreateTable(
                name: "work_order_services",
                columns: table => new
                {
                    work_order_id = table.Column<int>(type: "integer", nullable: false),
                    service_id = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_order_services", x => new { x.work_order_id, x.service_id });
                    table.CheckConstraint("ck_work_order_services_price", "price >= 0");
                    table.ForeignKey(
                        name: "FK_work_order_services_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_work_order_services_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Услуги в заказ-нарядах");

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    invoice_id = table.Column<int>(type: "integer", nullable: false),
                    paid_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    method = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    transaction_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.id);
                    table.CheckConstraint("ck_payments_amount", "amount > 0");
                    table.CheckConstraint("ck_payments_method", "method in ('Наличные', 'Карта', 'Перевод')");
                    table.CheckConstraint("ck_payments_status", "status in ('Проведён', 'Отменён')");
                    table.ForeignKey(
                        name: "FK_payments_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Платежи");

            migrationBuilder.InsertData(
                table: "car_brands",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Toyota" },
                    { 2, "Kia" }
                });

            migrationBuilder.InsertData(
                table: "car_categories",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Легковой" },
                    { 2, "Кроссовер" }
                });

            migrationBuilder.InsertData(
                table: "clients",
                columns: new[] { "id", "email", "full_name", "phone" },
                values: new object[,]
                {
                    { 1, "smirnov@example.com", "Смирнов Олег Викторович", "+79001112233" },
                    { 2, "orlova@example.com", "Орлова Анна Игоревна", "+79004445566" }
                });

            migrationBuilder.InsertData(
                table: "departments",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Слесарный цех" },
                    { 2, "Кузовной цех" }
                });

            migrationBuilder.InsertData(
                table: "repair_bays",
                columns: new[] { "id", "kind", "name" },
                values: new object[,]
                {
                    { 1, "Подъёмник", "Подъёмник 1" },
                    { 2, "Диагностический пост", "Диагностический пост 1" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Администратор" },
                    { 2, "Механик" },
                    { 3, "Руководитель" }
                });

            migrationBuilder.InsertData(
                table: "services",
                columns: new[] { "id", "base_price", "description", "duration_minutes", "name" },
                values: new object[,]
                {
                    { 1, 2500.00m, "Замена моторного масла и фильтра", 40, "Замена масла" },
                    { 2, 1800.00m, "Считывание кодов ошибок электронных систем", 30, "Компьютерная диагностика" },
                    { 3, 4200.00m, "Замена колодок передней оси", 90, "Замена тормозных колодок" }
                });

            migrationBuilder.InsertData(
                table: "specializations",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Моторист" },
                    { 2, "Автоэлектрик" }
                });

            migrationBuilder.InsertData(
                table: "suppliers",
                columns: new[] { "id", "email", "name", "phone" },
                values: new object[,]
                {
                    { 1, "sales@avtodetal.example", "ООО «АвтоДеталь»", "+78432001122" },
                    { 2, "karimov@example.com", "ИП Каримов", "+79037654321" }
                });

            migrationBuilder.InsertData(
                table: "cars",
                columns: new[] { "id", "car_brand_id", "car_category_id", "client_id", "color", "license_plate", "manufacture_year", "mileage", "model", "notes", "vin" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, "Белый", "А123ВС116", 2018, 82000, "Camry", "Комплектация Comfort", "JTM12345678900001" },
                    { 2, 2, 2, 1, "Серый", "К456ОР116", 2021, 34000, "Sportage", null, "XWE12345678900002" },
                    { 3, 1, 1, 2, "Синий", "М789ТТ116", 2016, 141000, "Corolla", null, "JTD12345678900003" }
                });

            migrationBuilder.InsertData(
                table: "parts",
                columns: new[] { "id", "min_quantity", "name", "purchase_price", "quantity", "sale_price", "sku", "supplier_id" },
                values: new object[,]
                {
                    { 1, 4, "Масляный фильтр", 400.00m, 12, 700.00m, "FIL-001", 1 },
                    { 2, 4, "Моторное масло 5W-30", 250.00m, 10, 450.00m, "OIL-5W30", 1 },
                    { 3, 4, "Тормозные колодки", 1800.00m, 1, 3200.00m, "PAD-001", 2 }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "full_name", "login", "password_hash", "role_id" },
                values: new object[,]
                {
                    { 1, "Иванова Мария Сергеевна", "admin", "100000.wZ5QTl7cXQpyQmhIilnjWQ==.DxQJHW9gnwkPa5t238+TMDp8s1VcYxrqyJYD+WXmPj0=", 1 },
                    { 2, "Петров Алексей Николаевич", "mechanic", "100000.iJdt0NmOptI11TdkBWEijQ==.62sQBvA3e2mayKIGA6Dvu9Z7dieB1zcJ66nM7VaOH24=", 2 },
                    { 3, "Сидоров Игорь Павлович", "mechanic2", "100000.Vvp5eKSeh5T+/+EILLQ21A==.ZfLw5GRwr6I1pxnouKpJBYYFXF9KhL/BkwXUXGhOhYQ=", 2 },
                    { 4, "Кузнецов Дмитрий Андреевич", "chief", "100000.D6Lp8Yio5nVp9seRgACV3Q==.TI4C15pFIO6Tx8659NCj6lc2qraufXW399Ly7eNRh0Q=", 3 }
                });

            migrationBuilder.InsertData(
                table: "mechanics",
                columns: new[] { "id", "department_id", "specialization_id", "user_id" },
                values: new object[,]
                {
                    { 1, 1, 1, 2 },
                    { 2, 2, 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "appointments",
                columns: new[] { "id", "car_id", "client_id", "mechanic_id", "repair_bay_id", "scheduled_at", "service_id", "status" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, 1, new DateTime(2026, 9, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, "Выполнена" },
                    { 2, 3, 2, 1, 2, new DateTime(2026, 10, 20, 11, 0, 0, 0, DateTimeKind.Unspecified), 2, "Запланирована" }
                });

            migrationBuilder.InsertData(
                table: "mechanic_schedules",
                columns: new[] { "id", "ends_at", "mechanic_id", "repair_bay_id", "starts_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 15, 18, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, new DateTime(2026, 9, 15, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2026, 10, 20, 18, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, new DateTime(2026, 10, 20, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2026, 9, 16, 18, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, new DateTime(2026, 9, 16, 9, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "mechanic_services",
                columns: new[] { "mechanic_id", "service_id" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "work_orders",
                columns: new[] { "id", "appointment_id", "created_at", "fault_description", "mechanic_id", "status" },
                values: new object[] { 1, 1, new DateTime(2026, 9, 15, 10, 20, 0, 0, DateTimeKind.Unspecified), "Пониженный уровень моторного масла, загрязнён масляный фильтр.", 1, "Завершён" });

            migrationBuilder.InsertData(
                table: "inspection_results",
                columns: new[] { "id", "brake_state", "electrical_state", "engine_state", "recommendations", "suspension_state", "work_order_id" },
                values: new object[] { 1, "Износа нет", "Ошибок электронных систем нет", "Уровень масла ниже нормы, фильтр загрязнён", "Повторить замену масла через 10 000 км", "Без замечаний", 1 });

            migrationBuilder.InsertData(
                table: "invoices",
                columns: new[] { "id", "amount_before_discount", "created_at", "discount", "status", "tax", "tax_rate", "total", "work_order_id" },
                values: new object[] { 1, 5000.00m, new DateTime(2026, 9, 15, 16, 0, 0, 0, DateTimeKind.Unspecified), 500.00m, "Оплачен", 900.00m, 20.00m, 5400.00m, 1 });

            migrationBuilder.InsertData(
                table: "reviews",
                columns: new[] { "id", "client_id", "comment", "created_at", "rating", "work_order_id" },
                values: new object[] { 1, 1, "Заменили масло быстро, объяснили состояние фильтра.", new DateTime(2026, 9, 16, 12, 0, 0, 0, DateTimeKind.Unspecified), 5, 1 });

            migrationBuilder.InsertData(
                table: "work_order_parts",
                columns: new[] { "part_id", "work_order_id", "quantity", "unit_price" },
                values: new object[,]
                {
                    { 1, 1, 1, 700.00m },
                    { 2, 1, 4, 450.00m }
                });

            migrationBuilder.InsertData(
                table: "work_order_services",
                columns: new[] { "service_id", "work_order_id", "price" },
                values: new object[] { 1, 1, 2500.00m });

            migrationBuilder.InsertData(
                table: "payments",
                columns: new[] { "id", "amount", "invoice_id", "method", "paid_at", "status", "transaction_number" },
                values: new object[] { 1, 5400.00m, 1, "Карта", new DateTime(2026, 9, 15, 16, 30, 0, 0, DateTimeKind.Unspecified), "Проведён", "TRX-20260915-0001" });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_car_id_client_id",
                table: "appointments",
                columns: new[] { "car_id", "client_id" });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_client_id",
                table: "appointments",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_mechanic_id",
                table: "appointments",
                column: "mechanic_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_repair_bay_id",
                table: "appointments",
                column: "repair_bay_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_scheduled_at",
                table: "appointments",
                column: "scheduled_at");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_service_id",
                table: "appointments",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_status",
                table: "appointments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_car_brands_name",
                table: "car_brands",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_car_categories_name",
                table: "car_categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cars_car_brand_id",
                table: "cars",
                column: "car_brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_cars_car_category_id",
                table: "cars",
                column: "car_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_cars_client_id",
                table: "cars",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_cars_license_plate",
                table: "cars",
                column: "license_plate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cars_mileage",
                table: "cars",
                column: "mileage");

            migrationBuilder.CreateIndex(
                name: "IX_cars_vin",
                table: "cars",
                column: "vin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clients_email",
                table: "clients",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clients_full_name",
                table: "clients",
                column: "full_name");

            migrationBuilder.CreateIndex(
                name: "IX_clients_phone",
                table: "clients",
                column: "phone");

            migrationBuilder.CreateIndex(
                name: "IX_departments_name",
                table: "departments",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inspection_results_work_order_id",
                table: "inspection_results",
                column: "work_order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_invoices_work_order_id",
                table: "invoices",
                column: "work_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_mechanic_schedules_mechanic_id",
                table: "mechanic_schedules",
                column: "mechanic_id");

            migrationBuilder.CreateIndex(
                name: "IX_mechanic_schedules_repair_bay_id",
                table: "mechanic_schedules",
                column: "repair_bay_id");

            migrationBuilder.CreateIndex(
                name: "IX_mechanic_services_service_id",
                table: "mechanic_services",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_mechanics_department_id",
                table: "mechanics",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_mechanics_specialization_id",
                table: "mechanics",
                column: "specialization_id");

            migrationBuilder.CreateIndex(
                name: "IX_mechanics_user_id",
                table: "mechanics",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_parts_quantity",
                table: "parts",
                column: "quantity");

            migrationBuilder.CreateIndex(
                name: "IX_parts_sku",
                table: "parts",
                column: "sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_parts_supplier_id",
                table: "parts",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_invoice_id",
                table: "payments",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_repair_bays_name",
                table: "repair_bays",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reviews_client_id",
                table: "reviews",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_work_order_id",
                table: "reviews",
                column: "work_order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_services_base_price",
                table: "services",
                column: "base_price");

            migrationBuilder.CreateIndex(
                name: "IX_services_duration_minutes",
                table: "services",
                column: "duration_minutes");

            migrationBuilder.CreateIndex(
                name: "IX_specializations_name",
                table: "specializations",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_login",
                table: "users",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_order_parts_part_id",
                table: "work_order_parts",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_order_services_service_id",
                table: "work_order_services",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_appointment_id",
                table: "work_orders",
                column: "appointment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_created_at",
                table: "work_orders",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_mechanic_id",
                table: "work_orders",
                column: "mechanic_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_orders_status",
                table: "work_orders",
                column: "status");

            migrationBuilder.Sql(
                """
                DO $body$
                DECLARE
                    table_name text;
                    max_id bigint;
                    sequence_name regclass;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY[
                        'roles', 'users', 'departments', 'specializations', 'mechanics',
                        'clients', 'car_brands', 'car_categories', 'cars', 'services',
                        'repair_bays', 'mechanic_schedules', 'appointments', 'work_orders',
                        'suppliers', 'parts', 'invoices', 'payments', 'inspection_results', 'reviews'
                    ]
                    LOOP
                        EXECUTE format('SELECT MAX(id) FROM %I', table_name) INTO max_id;
                        sequence_name := pg_get_serial_sequence(table_name, 'id')::regclass;
                        IF sequence_name IS NOT NULL AND max_id IS NOT NULL THEN
                            PERFORM setval(sequence_name, max_id, true);
                        END IF;
                    END LOOP;
                END
                $body$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inspection_results");

            migrationBuilder.DropTable(
                name: "mechanic_schedules");

            migrationBuilder.DropTable(
                name: "mechanic_services");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "reviews");

            migrationBuilder.DropTable(
                name: "work_order_parts");

            migrationBuilder.DropTable(
                name: "work_order_services");

            migrationBuilder.DropTable(
                name: "invoices");

            migrationBuilder.DropTable(
                name: "parts");

            migrationBuilder.DropTable(
                name: "work_orders");

            migrationBuilder.DropTable(
                name: "suppliers");

            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "cars");

            migrationBuilder.DropTable(
                name: "mechanics");

            migrationBuilder.DropTable(
                name: "repair_bays");

            migrationBuilder.DropTable(
                name: "services");

            migrationBuilder.DropTable(
                name: "car_brands");

            migrationBuilder.DropTable(
                name: "car_categories");

            migrationBuilder.DropTable(
                name: "clients");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "specializations");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}

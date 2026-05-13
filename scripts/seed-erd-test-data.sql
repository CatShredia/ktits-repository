-- Тестовые данные: поставщики, работники, worker_operations, изделия, заказы, спецификации, оборудование.
-- Таблицы users, materials, components (импорт Excel/CSV) не изменяются - только читаются для связей.
-- Повторный запуск: ON CONFLICT; работники с фамилией "Набор-NNN" добавляются один раз.
--
-- После миграций и импорта:
--   psql ... -f scripts/seed-erd-test-data.sql

BEGIN;

-- Поставщики (30 строк; не трогаем уже существующие имена - при совпадении строка пропускается)
INSERT INTO suppliers ("Name", "Address", "DeliveryDays")
SELECT
    'ООО Поставка-' || i::text,
    'г. Санкт-Петербург, промзона No.' || ((i % 5) + 1)::text,
    3 + (i % 12)
FROM generate_series(1, 30) AS i
ON CONFLICT ("Name") DO NOTHING;

-- Работники (40 строк; фамилия "Набор-NNN" - чтобы не дублировать при повторном запуске)
INSERT INTO workers ("LastName", "FirstMiddleName", "BirthDate", "HomeAddress", "Education", "Qualification")
SELECT
    'Набор-' || lpad(i::text, 3, '0'),
    'Иван Иванович',
    (DATE '1980-01-01' + (i * 47) % 6000)::date,
    'г. Казань, ул. Заводская, д. ' || (100 + i)::text,
    CASE WHEN i % 2 = 0 THEN 'Высшее' ELSE 'Среднее специальное' END,
    CASE (i % 5)
        WHEN 0 THEN 'Слесарь'
        WHEN 1 THEN 'Токарь'
        WHEN 2 THEN 'Сварщик'
        WHEN 3 THEN 'Контролер ОТК'
        ELSE 'Маляр'
    END
FROM generate_series(1, 40) AS i
WHERE NOT EXISTS (
    SELECT 1 FROM workers w WHERE w."LastName" = 'Набор-' || lpad(i::text, 3, '0')
);

-- Связь работник - операция (по ~2 операции на каждого "Набор-"; ~80 строк)
INSERT INTO worker_operations ("WorkerId", "OperationId")
SELECT w."Id", po."Id"
FROM workers w
CROSS JOIN LATERAL (
    SELECT p."Id"
    FROM production_operations p
    ORDER BY p."Id"
    LIMIT 2 OFFSET (
        abs(hashtext(w."Id"::text || w."LastName"))
        % GREATEST((SELECT COUNT(*)::int FROM production_operations) - 1, 0)
    )
) po
WHERE w."LastName" LIKE 'Набор-%'
ON CONFLICT ("WorkerId", "OperationId") DO NOTHING;

-- Типы оборудования (10)
INSERT INTO equipment_types ("Name")
SELECT v
FROM (VALUES
    ('Токарный станок'),
    ('Фрезерный станок'),
    ('Сверлильный станок'),
    ('Пресс гидравлический'),
    ('Сварочный пост'),
    ('Покрасочная камера'),
    ('Координатно-измерительная машина'),
    ('Кран-балка'),
    ('Компрессор'),
    ('Упаковочный стол')
) AS t(v)
ON CONFLICT ("Name") DO NOTHING;

-- Единицы оборудования (20 маркировок)
INSERT INTO equipment ("Marking", "EquipmentTypeName", "Characteristics")
SELECT
    'EQ-' || lpad(i::text, 4, '0'),
    et."Name",
    'Инв. No. завода ' || (1000 + i)::text || ', введено для демонстрации'
FROM generate_series(1, 20) AS i
JOIN LATERAL (
    SELECT "Name"
    FROM equipment_types
    ORDER BY "Name"
    OFFSET ((i - 1) % (SELECT COUNT(*)::int FROM equipment_types)) LIMIT 1
) et ON TRUE
ON CONFLICT ("Marking") DO NOTHING;

-- Изделия (20 шт.)
INSERT INTO products ("Name", "Dimensions")
SELECT
    'Изделие С-' || lpad(i::text, 4, '0'),
    (200 + i * 15)::text || 'x' || (150 + i * 10)::text || 'x' || (80 + i * 5)::text || ' mm'
FROM generate_series(1, 20) AS i
ON CONFLICT ("Name") DO NOTHING;

-- Спецификация сборки: цепочка изделий 1-2-...-20 (19 связей)
WITH pr AS (
    SELECT "Name", ROW_NUMBER() OVER (ORDER BY "Name") AS rn
    FROM products
    WHERE "Name" LIKE 'Изделие С-%'
)
INSERT INTO product_assembly_specs ("ParentProductName", "ChildProductName", "Quantity")
SELECT p1."Name", p2."Name", (1 + (abs(hashtext(p1."Name")) % 5))::decimal
FROM pr p1
JOIN pr p2 ON p2.rn = p1.rn + 1
ON CONFLICT ("ParentProductName", "ChildProductName") DO NOTHING;

-- Заказы (20 шт.; номера ЗК-2026-0001 - 0020)
WITH cust AS (
    SELECT "Login", ROW_NUMBER() OVER (ORDER BY "Login") AS rn
    FROM users
    WHERE "Role" = 'Заказчик'
),
mgr AS (
    SELECT "Login", ROW_NUMBER() OVER (ORDER BY "Login") AS rn
    FROM users
    WHERE "Role" = 'Менеджер'
),
pr AS (
    SELECT "Name", ROW_NUMBER() OVER (ORDER BY "Name") AS rn
    FROM products
    WHERE "Name" LIKE 'Изделие С-%'
),
cc AS (SELECT GREATEST(COUNT(*)::int, 1) AS c FROM cust),
mc AS (SELECT GREATEST(COUNT(*)::int, 1) AS c FROM mgr),
pc AS (SELECT GREATEST(COUNT(*)::int, 1) AS c FROM pr)
INSERT INTO customer_orders (
    "Number",
    "OrderName",
    "OrderDate",
    "ProductName",
    "CustomerLogin",
    "ManagerLogin",
    "EstimatedCost",
    "PlannedCompletionDate",
    "CustomerDrawings"
)
SELECT
    'ЗК-2026-' || lpad(i::text, 4, '0'),
    'Заказ на изготовление партии No.' || i::text,
    (DATE '2026-01-15' + (i % 40))::date,
    pr."Name",
    c."Login",
    m."Login",
    (50000 + i * 7500)::decimal,
    (DATE '2026-06-01' + (i % 60))::date,
    NULL
FROM generate_series(1, 20) AS i
JOIN pr ON pr.rn = ((i - 1) % (SELECT c FROM pc)) + 1
JOIN cust c ON c.rn = ((i - 1) % (SELECT c FROM cc)) + 1
LEFT JOIN mgr m ON m.rn = ((i - 1) % (SELECT c FROM mc)) + 1
WHERE EXISTS (SELECT 1 FROM pr)
ON CONFLICT ("Number") DO NOTHING;

-- Спецификация материалов: до 20 изделий × до 10 материалов из Excel (~200 строк, в ~10 раз больше исходных 2)
INSERT INTO product_material_specs ("ProductName", "MaterialId", "Quantity")
SELECT p."Name", m."Id", ROUND((0.5 + (abs(hashtext(p."Name" || m."Id"::text)) % 100))::numeric / 10, 2)
FROM (SELECT "Name" FROM products WHERE "Name" LIKE 'Изделие С-%' ORDER BY "Name" LIMIT 20) p
CROSS JOIN (SELECT "Id" FROM materials ORDER BY "Id" LIMIT 10) m
ON CONFLICT ("ProductName", "MaterialId") DO NOTHING;

-- Спецификация комплектующих: до 20 × до 10 (~200 строк)
INSERT INTO product_component_specs ("ProductName", "ComponentId", "Quantity")
SELECT p."Name", c."Id", ROUND((1 + (abs(hashtext(p."Name" || c."Id"::text)) % 20))::numeric / 2, 2)
FROM (SELECT "Name" FROM products WHERE "Name" LIKE 'Изделие С-%' ORDER BY "Name" LIMIT 20) p
CROSS JOIN (SELECT "Id" FROM components ORDER BY "Id" LIMIT 10) c
ON CONFLICT ("ProductName", "ComponentId") DO NOTHING;

-- Спецификация операций: изделия × операции каталога (до 20×5 = 100 строк)
INSERT INTO product_operation_specs ("ProductName", "OperationId", "SequenceNumber", "EquipmentTypeName", "DurationMinutes")
SELECT
    p."Name",
    op."Id",
    op.ord,
    et."Name",
    30 + (abs(hashtext(p."Name" || op."Id"::text)) % 240)
FROM (SELECT "Name" FROM products WHERE "Name" LIKE 'Изделие С-%' ORDER BY "Name" LIMIT 20) p
JOIN LATERAL (
    SELECT p2."Id", ROW_NUMBER() OVER (ORDER BY p2."Id") AS ord
    FROM production_operations p2
) op ON TRUE
LEFT JOIN LATERAL (
    SELECT e."Name"
    FROM equipment_types e
    ORDER BY e."Name"
    OFFSET (abs(hashtext(p."Name" || op."Id"::text)) % (SELECT GREATEST(COUNT(*)::int, 1) FROM equipment_types)) LIMIT 1
) et ON TRUE
ON CONFLICT ("ProductName", "OperationId", "SequenceNumber") DO NOTHING;

COMMIT;

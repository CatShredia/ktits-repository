-- Тестовые данные ERD + сессия 2 (заказы со статусами, история, замеры, цеха, сбои, ОТК).
-- Не изменяет: users, materials, components (импорт Excel/CSV).
-- Повторный запуск: ON CONFLICT / WHERE NOT EXISTS.
--
-- После миграций и импорта:
--   psql ... -f scripts/seed-erd-test-data.sql

BEGIN;

-- Поставщики (30 строк)
INSERT INTO suppliers ("Name", "Address", "DeliveryDays")
SELECT
    'ООО Поставка-' || i::text,
    'г. Санкт-Петербург, промзона No.' || ((i % 5) + 1)::text,
    3 + (i % 12)
FROM generate_series(1, 30) AS i
ON CONFLICT ("Name") DO NOTHING;

-- Работники (40 строк)
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

-- Связь работник — операция
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

-- Оборудование (20 маркировок)
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

-- Спецификация сборки
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

-- Заказы (20 шт.) — статусы WSR C2 для фильтров и ролей
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
pc AS (SELECT GREATEST(COUNT(*)::int, 1) AS c FROM pr),
ord AS (
    SELECT
        i,
        'ЗК-2026-' || lpad(i::text, 4, '0') AS num,
        CASE
            WHEN i <= 3 THEN 'Новый'
            WHEN i = 4 THEN 'Отменен'
            WHEN i <= 7 THEN 'Составление спецификации'
            WHEN i <= 9 THEN 'Подтверждение'
            WHEN i = 10 THEN 'Отклонен'
            WHEN i <= 12 THEN 'Закупка'
            WHEN i <= 14 THEN 'Производство'
            WHEN i <= 16 THEN 'Контроль'
            WHEN i <= 18 THEN 'Готов'
            ELSE 'Закрыт'
        END AS st
    FROM generate_series(1, 20) AS i
)
INSERT INTO customer_orders (
    "Number",
    "OrderName",
    "OrderDate",
    "ProductName",
    "CustomerLogin",
    "ManagerLogin",
    "EstimatedCost",
    "PlannedCompletionDate",
    "CustomerDrawings",
    "Status",
    "ProductDescription",
    "RejectionReason"
)
SELECT
    o.num,
    'Заказ на изготовление партии No.' || o.i::text,
    (DATE '2026-01-15' + (o.i % 40))::date,
    pr."Name",
    c."Login",
    CASE WHEN o.st IN ('Новый', 'Отменен', 'Отклонен') THEN NULL ELSE m."Login" END,
    CASE WHEN o.st IN ('Новый', 'Отменен') THEN NULL ELSE (50000 + o.i * 7500)::decimal END,
    CASE WHEN o.st IN ('Новый', 'Отменен', 'Отклонен') THEN NULL ELSE (DATE '2026-06-01' + (o.i % 60))::date END,
    NULL,
    o.st,
    'Конвейерная линия, исполнение по ТЗ No.' || o.i::text,
    CASE
        WHEN o.st = 'Отменен' THEN 'Отмена заказчиком до предоплаты'
        WHEN o.st = 'Отклонен' THEN 'Заказчик отказался от условий на этапе подтверждения'
        ELSE NULL
    END
FROM ord o
JOIN pr ON pr.rn = ((o.i - 1) % (SELECT c FROM pc)) + 1
JOIN cust c ON c.rn = ((o.i - 1) % (SELECT c FROM cc)) + 1
LEFT JOIN mgr m ON m.rn = ((o.i - 1) % (SELECT c FROM mc)) + 1
WHERE EXISTS (SELECT 1 FROM pr)
ON CONFLICT ("Number") DO UPDATE SET
    "Status" = EXCLUDED."Status",
    "ProductDescription" = EXCLUDED."ProductDescription",
    "RejectionReason" = EXCLUDED."RejectionReason",
    "ManagerLogin" = EXCLUDED."ManagerLogin",
    "EstimatedCost" = EXCLUDED."EstimatedCost",
    "PlannedCompletionDate" = EXCLUDED."PlannedCompletionDate";

-- Старые заказы без статуса
UPDATE customer_orders
SET "Status" = 'Новый',
    "ProductDescription" = COALESCE(NULLIF("ProductDescription", ''), 'Описание не заполнено')
WHERE "Status" IS NULL OR "Status" = '';

-- Замеры размеров (по 2 на заказы 1–10)
INSERT INTO order_dimensions ("OrderNumber", "Description", "Unit", "Value")
SELECT
    'ЗК-2026-' || lpad(i::text, 4, '0'),
    d.descr,
    d.unit,
    d.val
FROM generate_series(1, 10) AS i
CROSS JOIN (VALUES
    ('Длина ленты', 'м', 12.5::decimal),
    ('Ширина рамы', 'мм', 800::decimal)
) AS d(descr, unit, val)
WHERE EXISTS (
    SELECT 1 FROM customer_orders o WHERE o."Number" = 'ЗК-2026-' || lpad(i::text, 4, '0')
)
AND NOT EXISTS (
    SELECT 1 FROM order_dimensions od
    WHERE od."OrderNumber" = 'ЗК-2026-' || lpad(i::text, 4, '0')
      AND od."Description" = d.descr
);

-- История статусов (текущий статус по каждому заказу ЗК-2026-*)
INSERT INTO order_status_history ("OrderNumber", "Status", "ChangedAt", "ChangedByLogin", "Comment")
SELECT
    o."Number",
    o."Status",
    TIMESTAMPTZ '2026-05-01 10:00:00+00' + ((o.seq % 48) || ' hours')::interval,
    COALESCE(o."ManagerLogin", o."CustomerLogin"),
    'Текущий статус (seed)'
FROM (
    SELECT
        "Number",
        "Status",
        "ManagerLogin",
        "CustomerLogin",
        (substring("Number" from 9))::int AS seq
    FROM customer_orders
    WHERE "Number" LIKE 'ЗК-2026-%'
) o
WHERE NOT EXISTS (
    SELECT 1 FROM order_status_history h
    WHERE h."OrderNumber" = o."Number" AND h."Status" = o."Status"
);

-- Полная цепочка истории для заказа ЗК-2026-0015
INSERT INTO order_status_history ("OrderNumber", "Status", "ChangedAt", "ChangedByLogin", "Comment")
SELECT v.num, v.st, v.ts, v.login, v.cmt
FROM (VALUES
    ('ЗК-2026-0015', 'Новый',                    TIMESTAMPTZ '2026-04-01 08:00:00+00', NULL::varchar, 'Создан'),
    ('ЗК-2026-0015', 'Составление спецификации', TIMESTAMPTZ '2026-04-03 09:00:00+00', NULL::varchar, 'Принят менеджером'),
    ('ЗК-2026-0015', 'Подтверждение',            TIMESTAMPTZ '2026-04-10 11:00:00+00', NULL::varchar, 'Согласовано с заказчиком'),
    ('ЗК-2026-0015', 'Закупка',                  TIMESTAMPTZ '2026-04-15 12:00:00+00', NULL::varchar, 'Материалы заказаны'),
    ('ЗК-2026-0015', 'Производство',             TIMESTAMPTZ '2026-05-01 07:00:00+00', NULL::varchar, 'Передано в цех'),
    ('ЗК-2026-0015', 'Контроль',                 TIMESTAMPTZ '2026-05-10 14:00:00+00', NULL::varchar, 'ОТК')
) AS v(num, st, ts, login, cmt)
WHERE EXISTS (SELECT 1 FROM customer_orders o WHERE o."Number" = v.num)
AND NOT EXISTS (
    SELECT 1 FROM order_status_history h
    WHERE h."OrderNumber" = v.num AND h."Status" = v.st AND h."ChangedAt" = v.ts
);

-- Спецификации изделий
INSERT INTO product_material_specs ("ProductName", "MaterialId", "Quantity")
SELECT p."Name", m."Id", ROUND((0.5 + (abs(hashtext(p."Name" || m."Id"::text)) % 100))::numeric / 10, 2)
FROM (SELECT "Name" FROM products WHERE "Name" LIKE 'Изделие С-%' ORDER BY "Name" LIMIT 20) p
CROSS JOIN (SELECT "Id" FROM materials ORDER BY "Id" LIMIT 10) m
ON CONFLICT ("ProductName", "MaterialId") DO NOTHING;

INSERT INTO product_component_specs ("ProductName", "ComponentId", "Quantity")
SELECT p."Name", c."Id", ROUND((1 + (abs(hashtext(p."Name" || c."Id"::text)) % 20))::numeric / 2, 2)
FROM (SELECT "Name" FROM products WHERE "Name" LIKE 'Изделие С-%' ORDER BY "Name" LIMIT 20) p
CROSS JOIN (SELECT "Id" FROM components ORDER BY "Id" LIMIT 10) c
ON CONFLICT ("ProductName", "ComponentId") DO NOTHING;

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

-- Цеха (изображения планов подгружает API при первом запуске, если таблица пуста)
INSERT INTO workshops ("Name", "FloorPlanImage")
SELECT v, NULL
FROM (VALUES
    ('Сборочный цех'),
    ('Заготовительный цех'),
    ('Покрасочный цех'),
    ('Механический цех'),
    ('Упаковочный цех')
) AS t(v)
ON CONFLICT ("Name") DO NOTHING;

-- Демо-значки на планах
INSERT INTO workshop_layout_items ("WorkshopId", "IconType", "X", "Y")
SELECT w."Id", v.icon, v.x, v.y
FROM workshops w
JOIN (VALUES
    ('Сборочный цех', 'FireExtinguisher', 0.15, 0.20),
    ('Сборочный цех', 'FirstAid', 0.25, 0.20),
    ('Сборочный цех', 'Exit', 0.85, 0.50),
    ('Сборочный цех', 'Equipment', 0.50, 0.45),
    ('Механический цех', 'Equipment', 0.40, 0.55),
    ('Механический цех', 'FireExtinguisher', 0.10, 0.15)
) AS v(shop, icon, x, y) ON w."Name" = v.shop
WHERE NOT EXISTS (
    SELECT 1 FROM workshop_layout_items li WHERE li."WorkshopId" = w."Id"
);

-- Сбои оборудования (5 записей, мастер)
WITH foreman AS (
    SELECT "Login" FROM users WHERE "Role" = 'Мастер' ORDER BY "Login" LIMIT 1
)
INSERT INTO equipment_failures ("EquipmentMarking", "StartedAt", "EndedAt", "Reason", "RegisteredByLogin")
SELECT
    'EQ-' || lpad(i::text, 4, '0'),
    TIMESTAMPTZ '2026-05-01 08:00:00+00' + (i * 6 || ' hours')::interval,
    CASE WHEN i % 2 = 0
        THEN TIMESTAMPTZ '2026-05-01 12:00:00+00' + (i * 6 || ' hours')::interval
        ELSE NULL END,
    'Демо-сбой No.' || i::text || ': отказ узла привода',
    f."Login"
FROM generate_series(1, 5) AS i
CROSS JOIN foreman f
WHERE EXISTS (SELECT 1 FROM equipment e WHERE e."Marking" = 'EQ-' || lpad(i::text, 4, '0'))
  AND EXISTS (SELECT 1 FROM foreman)
  AND NOT EXISTS (
      SELECT 1 FROM equipment_failures ef
      WHERE ef."EquipmentMarking" = 'EQ-' || lpad(i::text, 4, '0')
  );

-- Контроль качества (заказы в статусе «Контроль»)
WITH foreman AS (
    SELECT "Login" FROM users WHERE "Role" = 'Мастер' ORDER BY "Login" LIMIT 1
),
qc_orders AS (
    SELECT "Number" FROM customer_orders WHERE "Status" = 'Контроль' LIMIT 3
)
INSERT INTO order_quality_checks ("OrderNumber", "ParameterName", "Grade", "Comment", "CheckedAt", "CheckedByLogin")
SELECT
    o."Number",
    p.param,
    p.grade,
    p.cmt,
    TIMESTAMPTZ '2026-05-12 10:00:00+00',
    f."Login"
FROM qc_orders o
CROSS JOIN foreman f
CROSS JOIN (VALUES
    ('Свободный ход деталей', '+', NULL::varchar),
    ('Зазоры в соединениях', '+', NULL::varchar),
    ('Покрытие поверхности', '-', 'Требуется доработка покраски')
) AS p(param, grade, cmt)
WHERE EXISTS (SELECT 1 FROM foreman)
AND NOT EXISTS (
    SELECT 1 FROM order_quality_checks q
    WHERE q."OrderNumber" = o."Number" AND q."ParameterName" = p.param
);

COMMIT;

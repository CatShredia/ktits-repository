-- ============================================
-- ВАЖНО: Замените UUID ниже на ID вашего пользователя
-- ============================================
-- Чтобы получить ваш user_id, выполните:
-- SELECT id FROM auth.users LIMIT 1;
-- ============================================
DO $$
DECLARE
    test_user_id UUID := '00000000-0000-0000-0000-000000000000';

-- ЗАМЕНИТЕ НА ВАШ UUID
BEGIN
    -- ============================================
    -- ОЧИСТКА ДАННЫХ (в обратном порядке FK)
    -- ============================================
    -- 1. Сначала удаляем продукты (зависят от categories и users)
    DELETE FROM
        products;

-- 2. Удаляем уведомления (зависят от users)
DELETE FROM
    notifications;

-- 3. Удаляем категории
DELETE FROM
    product_categories;

-- Примечание: таблица users (auth.users) управляется Supabase Auth,
-- поэтому её не очищаем напрямую
-- ============================================
-- СБРОС СЧЁТЧИКОВ ID (опционально)
-- ============================================
ALTER SEQUENCE product_categories_id_seq RESTART WITH 1;

ALTER SEQUENCE products_id_seq RESTART WITH 1;

ALTER SEQUENCE notifications_id_seq RESTART WITH 1;

-- ============================================
-- ЗАПОЛНЕНИЕ ТЕСТОВЫМИ ДАННЫМИ
-- ============================================
-- --------------------------------------------
-- 1. Категории продуктов (5 записей)
-- --------------------------------------------
INSERT INTO
    product_categories (id, created_at, name, image)
VALUES
    (
        1,
        NOW(),
        'Электроника',
        'https://images.unsplash.com/photo-1498049794561-7780e7231661?w=400'
    ),
    (
        2,
        NOW(),
        'Одежда',
        'https://images.unsplash.com/photo-1445205170230-053b83016050?w=400'
    ),
    (
        3,
        NOW(),
        'Дом и сад',
        'https://images.unsplash.com/photo-1484101403633-562f891dc89a?w=400'
    ),
    (
        4,
        NOW(),
        'Спорт',
        'https://images.unsplash.com/photo-1517836357463-d25dfeac3438?w=400'
    ),
    (
        5,
        NOW(),
        'Книги',
        'https://images.unsplash.com/photo-1495446815901-a7297e633e8d?w=400'
    );

-- --------------------------------------------
-- 2. Продукты (15 записей, по 3 на категорию)
-- --------------------------------------------
INSERT INTO
    products (
        id,
        user_id,
        category_id,
        name,
        image,
        description,
        price_cents,
        currency,
        stock,
        is_active,
        created_at
    )
VALUES
    -- Электроника (category_id = 1)
    (
        1,
        test_user_id,
        1,
        'Смартфон XYZ Pro',
        'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400',
        'Мощный смартфон с отличной камерой',
        79900,
        'RUB',
        50,
        true,
        NOW()
    ),
    (
        2,
        test_user_id,
        1,
        'Ноутбук UltraBook 15',
        'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400',
        'Лёгкий и производительный ноутбук',
        129900,
        'RUB',
        25,
        true,
        NOW()
    ),
    (
        3,
        test_user_id,
        1,
        'Беспроводные наушники',
        'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400',
        'Наушники с шумоподавлением',
        15900,
        'RUB',
        100,
        true,
        NOW()
    ),
    -- Одежда (category_id = 2)
    (
        4,
        test_user_id,
        2,
        'Футболка хлопковая',
        'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400',
        'Классическая футболка из 100% хлопка',
        2500,
        'RUB',
        200,
        true,
        NOW()
    ),
    (
        5,
        test_user_id,
        2,
        'Джинсы Classic Fit',
        'https://images.unsplash.com/photo-1542272454315-4c01d7abdf4a?w=400',
        'Универсальные джинсы на каждый день',
        5900,
        'RUB',
        150,
        true,
        NOW()
    ),
    (
        6,
        test_user_id,
        2,
        'Куртка осенняя',
        'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400',
        'Стильная куртка для прохладной погоды',
        12900,
        'RUB',
        75,
        true,
        NOW()
    ),
    -- Дом и сад (category_id = 3)
    (
        7,
        test_user_id,
        3,
        'Набор постельного белья',
        'https://images.unsplash.com/photo-1522771753035-4a5042305a63?w=400',
        'Комплект из бязи, евро размер',
        4500,
        'RUB',
        80,
        true,
        NOW()
    ),
    (
        8,
        test_user_id,
        3,
        'Ваза декоративная',
        'https://images.unsplash.com/photo-1581783342308-f792ca11df53?w=400',
        'Керамическая ваза в современном стиле',
        3200,
        'RUB',
        40,
        true,
        NOW()
    ),
    (
        9,
        test_user_id,
        3,
        'Светильник настольный',
        'https://images.unsplash.com/photo-1507473888900-52e1ad14592d?w=400',
        'LED-светильник с регулировкой яркости',
        2800,
        'RUB',
        60,
        true,
        NOW()
    ),
    -- Спорт (category_id = 4)
    (
        10,
        test_user_id,
        4,
        'Гантели разборные',
        'https://images.unsplash.com/photo-1571902943202-507ec2618e8f?w=400',
        'Набор гантелей от 2 до 20 кг',
        8900,
        'RUB',
        30,
        true,
        NOW()
    ),
    (
        11,
        test_user_id,
        4,
        'Коврик для йоги',
        'https://images.unsplash.com/photo-1601925260368-ae2f83cf8b7f?w=400',
        'Противоскользящий коврик 6мм',
        2100,
        'RUB',
        120,
        true,
        NOW()
    ),
    (
        12,
        test_user_id,
        4,
        'Бутылка для воды',
        'https://images.unsplash.com/photo-1602143407151-011141951e7a?w=400',
        'Спортивная бутылка 750мл',
        1200,
        'RUB',
        200,
        true,
        NOW()
    ),
    -- Книги (category_id = 5)
    (
        13,
        test_user_id,
        5,
        'Чистый код',
        'https://images.unsplash.com/photo-1532012197267-da84d127e765?w=400',
        'Роберт Мартин — руководство по написанию кода',
        1800,
        'RUB',
        45,
        true,
        NOW()
    ),
    (
        14,
        test_user_id,
        5,
        'Изучаем Python',
        'https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=400',
        'Марк Лутц — полное руководство',
        3200,
        'RUB',
        35,
        true,
        NOW()
    ),
    (
        15,
        test_user_id,
        5,
        'Алгоритмы',
        'https://images.unsplash.com/photo-1589829545856-d10d557cf95f?w=400',
        'Кормен, Лейзерсон — фундаментальный труд',
        4500,
        'RUB',
        25,
        true,
        NOW()
    );

-- --------------------------------------------
-- 3. Уведомления (10 записей)
-- --------------------------------------------
INSERT INTO
    notifications (
        id,
        created_at,
        user_id,
        title,
        body,
        status,
        read_at
    )
VALUES
    (
        1,
        NOW(),
        test_user_id,
        'Заказ оформлен',
        'Ваш заказ #1234 успешно оформлен',
        'success',
        false
    ),
    (
        2,
        NOW(),
        test_user_id,
        'Скидка 20%',
        'Только сегодня скидка на электронику',
        'info',
        false
    ),
    (
        3,
        NOW(),
        test_user_id,
        'Новый товар',
        'Появился новый товар в категории "Одежда"',
        'info',
        false
    ),
    (
        4,
        NOW(),
        test_user_id,
        'Напоминание',
        'Не забудьте завершить оформление заказа',
        'warning',
        false
    ),
    (
        5,
        NOW(),
        test_user_id,
        'Доставка',
        'Ваш заказ передан в службу доставки',
        'success',
        true
    ),
    (
        6,
        NOW(),
        test_user_id,
        'Акция',
        'Успейте купить по специальной цене',
        'promo',
        false
    ),
    (
        7,
        NOW(),
        test_user_id,
        'Бонусы',
        'Вам начислено 500 бонусных баллов',
        'success',
        true
    ),
    (
        8,
        NOW(),
        test_user_id,
        'Обновление',
        'Приложение обновлено до версии 2.0',
        'info',
        true
    ),
    (
        9,
        NOW(),
        test_user_id,
        'Отзыв',
        'Оставьте отзыв о покупке',
        'question',
        false
    ),
    (
        10,
        NOW(),
        test_user_id,
        'Событие',
        'Через 3 дня начнётся распродажа',
        'event',
        false
    );

END $$;

-- ============================================
-- ПРОВЕРКА РЕЗУЛЬТАТОВ
-- ============================================
SELECT
    'product_categories' AS table_name,
    COUNT(*) AS record_count
FROM
    product_categories
UNION
ALL
SELECT
    'products',
    COUNT(*)
FROM
    products
UNION
ALL
SELECT
    'notifications',
    COUNT(*)
FROM
    notifications;
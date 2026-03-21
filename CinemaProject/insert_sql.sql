-- 1. Очистка данных и сброс последовательностей (ID)
-- TRUNCATE быстрее DELETE и автоматически сбрасывает счетчики при использовании RESTART IDENTITY.
-- CASCADE нужен, чтобы очистить зависимые таблицы (например, Ratings и Logins) вместе с основными.
TRUNCATE TABLE public."Ratings",
public."Logins",
public."Films",
public."Users",
public."Genres" RESTART IDENTITY CASCADE;

-- 2. Вставка тестовых данных
-- Таблица: Genres (Жанры)
INSERT INTO
    public."Genres" ("Name", "Description")
VALUES
    (
        'Sci-Fi',
        'Научная фантастика о будущем и технологиях'
    ),
    (
        'Drama',
        'Глубокие драматические истории о человеческих судьбах'
    ),
    (
        'Action',
        'Динамичные фильмы с погонями и взрывами'
    ),
    (
        'Comedy',
        'Легкие комедии для поднятия настроения'
    ),
    (
        'Horror',
        'Фильмы ужасов, от которых стынет кровь'
    );

-- Таблица: Users (Пользователи)
-- Обратите внимание: Gender должен быть NOT NULL
INSERT INTO
    public."Users" (
        "Surname",
        "Name",
        "Description",
        "Gender",
        "Email"
    )
VALUES
    (
        'Иванов',
        'Алексей',
        'Любитель научной фантастики',
        'Male',
        'alexey.ivanov@example.com'
    ),
    (
        'Петрова',
        'Мария',
        'Кинокритик со стажем',
        'Female',
        'maria.petrova@example.com'
    ),
    (
        'Сидоров',
        'Дмитрий',
        'Фанат экшена',
        'Male',
        'dmitry.sidorov@example.com'
    ),
    (
        'Смирнова',
        'Елена',
        'Ценитель классики',
        'Female',
        'elena.smirnova@example.com'
    ),
    (
        'Кузнецов',
        'Игорь',
        'Смотрю всё подряд',
        'Male',
        'igor.kuznetsov@example.com'
    );

-- Таблица: Films (Фильмы)
-- Связываем с существующими GenreId (1-5) и AuthorId (1-5)
INSERT INTO
    public."Films" (
        "Name",
        "Description",
        "ReleaseDate",
        "GenreId",
        "ImageUrl",
        "AuthorId"
    )
VALUES
    (
        'Интерстеллар 2',
        'Продолжение эпического путешествия сквозь червоточины',
        '2024-05-15 10:00:00+00',
        1,
        'https://example.com/interstellar2.jpg',
        1
    ),
    (
        'Тихая гавань',
        'История о любви в маленьком городке',
        '2023-11-20 12:00:00+00',
        2,
        'https://example.com/haven.jpg',
        2
    ),
    (
        'Быстрее пули',
        'Агент должен остановить террористов за 24 часа',
        '2024-01-10 09:00:00+00',
        3,
        'https://example.com/bullet.jpg',
        3
    ),
    (
        'День сурка 2',
        'Герой снова застревает во времени, но теперь в лесу',
        '2023-08-05 14:30:00+00',
        4,
        'https://example.com/groundhog2.jpg',
        4
    ),
    (
        'Тень в углу',
        'Дом с привидениями хранит мрачную тайну',
        '2024-10-31 23:00:00+00',
        5,
        'https://example.com/shadow.jpg',
        5
    );

-- Таблица: Logins (Логины)
-- Связываем с UserId (1-5). Пароли указаны как хэши (для примера просто строки)
INSERT INTO
    public."Logins" ("LoginValue", "PasswordHash", "UserId")
VALUES
    (
        'alex_ivanov',
        '$2b$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/X4.G.2.2.2.2.2.2.2',
        1
    ),
    (
        'masha_p',
        '$2b$12$XYZabc1234567890abcdefghijklmnopqrstuvwxyzABCDEF',
        2
    ),
    (
        'dimon_action',
        '$2b$12$1111111111111111111111111111111111111111111111111111',
        3
    ),
    (
        'lena_cinema',
        '$2b$12$2222222222222222222222222222222222222222222222222222',
        4
    ),
    (
        'igor_viewer',
        '$2b$12$3333333333333333333333333333333333333333333333333333',
        5
    );

-- Таблица: Ratings (Оценки)
-- Связываем с FilmId и AuthorId. Value от 1 до 10.
INSERT INTO
    public."Ratings" ("Value", "FilmId", "AuthorId")
VALUES
    (10, 1, 2),
    -- Мария оценила Интерстеллар на 10
    (8, 1, 3),
    -- Дмитрий оценил Интерстеллар на 8
    (5, 2, 1),
    -- Алексей оценил Тихую гавань на 5
    (9, 3, 4),
    -- Елена оценила Быстрее пули на 9
    (7, 5, 5),
    -- Игорь оценил Тень в углу на 7
    (6, 4, 1),
    -- Алексей оценил День сурка 2 на 6
    (10, 3, 2);

-- Мария оценила Быстрее пули на 10
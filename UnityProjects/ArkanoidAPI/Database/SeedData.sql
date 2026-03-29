-- Arkanoid Shop - Тестовые данные для таблицы Skins
-- Выполнить после применения миграций

-- Очистка таблицы (опционально)
-- DELETE FROM "Skins";

-- Сброс автоинкремента
-- ALTER SEQUENCE "Skins_Id_seq" RESTART WITH 1;

-- ============================================
-- ПЛАТФОРМЫ (Platform Skins)
-- ============================================

-- Common (Обычные)
INSERT INTO "Skins" ("Name", "Description", "SkinType", "Rarity", "Price", "TexturePath", "PrefabPath", "IsStarter", "IsActive", "CreatedAt")
VALUES 
('Стандартная', 'Обычная серая платформа', 0, 0, 0, 'Textures/Platforms/standard', 'Prefabs/Platforms/standard', true, true, NOW()),
('Неоновая', 'Платформа с неоновой подсветкой', 0, 0, 100, 'Textures/Platforms/neon', 'Prefabs/Platforms/neon', false, true, NOW()),
('Металлическая', 'Прочная металлическая платформа', 0, 0, 120, 'Textures/Platforms/metal', 'Prefabs/Platforms/metal', false, true, NOW());

-- Uncommon (Необычные)
INSERT INTO "Skins" ("Name", "Description", "SkinType", "Rarity", "Price", "TexturePath", "PrefabPath", "IsStarter", "IsActive", "CreatedAt")
VALUES 
('Деревянная', 'Платформа из дерева', 0, 1, 200, 'Textures/Platforms/wood', 'Prefabs/Platforms/wood', false, true, NOW()),
('Каменная', 'Тяжёлая каменная платформа', 0, 1, 250, 'Textures/Platforms/stone', 'Prefabs/Platforms/stone', false, true, NOW()),
('Ледяная', 'Холодная ледяная платформа', 0, 1, 280, 'Textures/Platforms/ice', 'Prefabs/Platforms/ice', false, true, NOW());

-- Rare (Редкие)
INSERT INTO "Skins" ("Name", "Description", "SkinType", "Rarity", "Price", "TexturePath", "PrefabPath", "IsStarter", "IsActive", "CreatedAt")
VALUES 
('Огненная', 'Платформа объятая пламенем', 0, 2, 400, 'Textures/Platforms/fire', 'Prefabs/Platforms/fire', false, true, NOW()),
('Золотая', 'Богатая золотая платформа', 0, 2, 500, 'Textures/Platforms/gold', 'Prefabs/Platforms/gold', false, true, NOW()),
('Электрическая', 'Платформа с разрядами молний', 0, 2, 550, 'Textures/Platforms/electric', 'Prefabs/Platforms/electric', false, true, NOW());

-- Epic (Эпические)
INSERT INTO "Skins" ("Name", "Description", "SkinType", "Rarity", "Price", "TexturePath", "PrefabPath", "IsStarter", "IsActive", "CreatedAt")
VALUES 
('Кибер', 'Футуристическая кибер-платформа', 0, 3, 800, 'Textures/Platforms/cyber', 'Prefabs/Platforms/cyber', false, true, NOW()),
('Тёмная материя', 'Платформа из тёмной материи', 0, 3, 900, 'Textures/Platforms/darkmatter', 'Prefabs/Platforms/darkmatter', false, true, NOW()),
('Космическая', 'Платформа с звёздной текстурой', 0, 3, 1000, 'Textures/Platforms/space', 'Prefabs/Platforms/space', false, true, NOW());

-- Legendary (Легендарные)
INSERT INTO "Skins" ("Name", "Description", "SkinType", "Rarity", "Price", "TexturePath", "PrefabPath", "IsStarter", "IsActive", "CreatedAt")
VALUES 
('Драконья', 'Платформа с чешуёй дракона', 0, 4, 1500, 'Textures/Platforms/dragon', 'Prefabs/Platforms/dragon', false, true, NOW()),
('Божественная', 'Священная платформа богов', 0, 4, 2000, 'Textures/Platforms/divine', 'Prefabs/Platforms/divine', false, true, NOW()),
('Радужная', 'Переливающаяся всеми цветами', 0, 4, 1800, 'Textures/Platforms/rainbow', 'Prefabs/Platforms/rainbow', false, true, NOW());

-- ============================================
-- МЯЧИ (Ball Skins)
-- ============================================

-- Common (Обычные)
INSERT INTO "Skins" ("Name", "Description", "SkinType", "Rarity", "Price", "TexturePath", "PrefabPath", "IsStarter", "IsActive", "CreatedAt")
VALUES 
('Классический', 'Обычный белый мяч', 1, 0, 0, 'Textures/Balls/classic', 'Prefabs/Balls/classic', true, true, NOW()),
('Красный', 'Простой красный мяч', 1, 0, 100, 'Textures/Balls/red', 'Prefabs/Balls/red', false, true, NOW()),
('Синий', 'Простой синий мяч', 1, 0, 100, 'Textures/Balls/blue', 'Prefabs/Balls/blue', false, true, NOW());

-- Uncommon (Необычные)
INSERT INTO "Skins" ("Name", "Description", "SkinType", "Rarity", "Price", "TexturePath", "PrefabPath", "IsStarter", "IsActive", "CreatedAt")
VALUES 
('Зелёный', 'Ярко-зелёный мяч', 1, 1, 200, 'Textures/Balls/green', 'Prefabs/Balls/green', false, true, NOW()),
('Жёлтый', 'Солнечный жёлтый мяч', 1, 1, 220, 'Textures/Balls/yellow', 'Prefabs/Balls/yellow', false, true, NOW()),
('Фиолетовый', 'Таинственный фиолетовый мяч', 1, 1, 250, 'Textures/Balls/purple', 'Prefabs/Balls/purple', false, true, NOW());

-- Rare (Редкие)
INSERT INTO "Skins" ("Name", "Description", "SkinType", "Rarity", "Price", "TexturePath", "PrefabPath", "IsStarter", "IsActive", "CreatedAt")
VALUES 
('Золотой', 'Блестящий золотой мяч', 1, 2, 450, 'Textures/Balls/gold', 'Prefabs/Balls/gold', false, true, NOW()),
('Огненный', 'Пылающий огненный мяч', 1, 2, 500, 'Textures/Balls/fireball', 'Prefabs/Balls/fireball', false, true, NOW()),
('Ледяной', 'Замерзший ледяной мяч', 1, 2, 500, 'Textures/Balls/iceball', 'Prefabs/Balls/iceball', false, true, NOW());

-- Epic (Эпические)
INSERT INTO "Skins" ("Name", "Description", "SkinType", "Rarity", "Price", "TexturePath", "PrefabPath", "IsStarter", "IsActive", "CreatedAt")
VALUES 
('Молния', 'Мяч с разрядами энергии', 1, 3, 850, 'Textures/Balls/lightning', 'Prefabs/Balls/lightning', false, true, NOW()),
('Галактика', 'Мяч с текстурой галактики', 1, 3, 950, 'Textures/Balls/galaxy', 'Prefabs/Balls/galaxy', false, true, NOW()),
('Ядерный', 'Радиоактивный светящийся мяч', 1, 3, 1000, 'Textures/Balls/nuclear', 'Prefabs/Balls/nuclear', false, true, NOW());

-- Legendary (Легендарные)
INSERT INTO "Skins" ("Name", "Description", "SkinType", "Rarity", "Price", "TexturePath", "PrefabPath", "IsStarter", "IsActive", "CreatedAt")
VALUES 
('Драконий', 'Мяч с силой дракона', 1, 4, 1600, 'Textures/Balls/dragonball', 'Prefabs/Balls/dragonball', false, true, NOW()),
('Божественный', 'Священный мяч богов', 1, 4, 2200, 'Textures/Balls/divineball', 'Prefabs/Balls/divineball', false, true, NOW()),
('Радужный', 'Переливающийся мяч', 1, 4, 1900, 'Textures/Balls/rainbowball', 'Prefabs/Balls/rainbowball', false, true, NOW()),
('Тёмная звезда', 'Мяч из тёмной звезды', 1, 4, 2500, 'Textures/Balls/darkstar', 'Prefabs/Balls/darkstar', false, true, NOW());

-- ============================================
-- Проверка данных
-- ============================================

-- Посчитать количество скинов по типам
SELECT "SkinType", COUNT(*) as "Count" 
FROM "Skins" 
GROUP BY "SkinType";

-- Посчитать количество скинов по редкости
SELECT "Rarity", COUNT(*) as "Count" 
FROM "Skins" 
GROUP BY "Rarity" 
ORDER BY "Rarity";

-- Показать все скины
SELECT "Id", "Name", "SkinType", "Rarity", "Price", "IsStarter", "IsActive" 
FROM "Skins" 
ORDER BY "SkinType", "Rarity", "Price";

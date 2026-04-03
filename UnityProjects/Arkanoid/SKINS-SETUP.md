# Инструкция по настройке системы скинов

## 1. Добавить SkinManager на сцену

1. Откройте сцену `Assets/Scenes/MainScene.unity`
2. Создайте пустой GameObject: `GameObject → Create Empty`
3. Переименуйте его в `SkinManager`
4. Добавьте компонент `SkinManager` (скрипт уже в проекте)
5. Компонент автоматически установит `DontDestroyOnLoad`

## 2. Настроить спрайты скинов в SkinManager

В инспекторе компонента `SkinManager`:

### Массив `Skin Sprites` (skinSprites)

Для каждого скина добавьте элемент в массив:

- **spriteName**: ключ спрайта — **должен совпадать с `PrefabPath` из БД** (например `Button-Photoroom_1`, `BasicArkanoidPack_10`)
- **sprite**: перетащите соответствующий Sprite из проекта

### Массив `Skin Name Aliases` (skinNameAliases)

Это маппинг имён из БД на ключи спрайтов. Нужен для разрешения старых записей в PlayerPrefs.

Для каждого скина добавьте элемент:

- **db Skin Name**: имя скина из БД (поле `Name`, например `платформа 3`)
- **sprite Key**: ключ спрайта (совпадает с `spriteName` в массиве выше, например `Button-Photoroom_1`)

### Пример полной настройки для всех скинов:

#### Skin Sprites:

| spriteName (ключ)      | sprite                               |
| ---------------------- | ------------------------------------ |
| `BasicArkanoidPack_14` | BasicArkanoidPack_14 (из спрайтшита) |
| `Button-Photoroom_1`   | Button-Photoroom_1 (из спрайтшита)   |
| `BasicArkanoidPack_10` | BasicArkanoidPack_10 (из спрайтшита) |
| ...                    | ...                                  |

#### Skin Name Aliases:

| db Skin Name  | sprite Key                             |
| ------------- | -------------------------------------- |
| `платформа 1` | `BasicArkanoidPack_14`                 |
| `платформа 2` | `Button-Photoroom_0`                   |
| `платформа 3` | `Button-Photoroom_1`                   |
| `платформа 4` | `Button-Photoroom_2`                   |
| `платформа 5` | `Button-Photoroom_3`                   |
| `шар 0`       | `BasicArkanoidPack_10`                 |
| `шар 1`       | `i (2)-no-bg-preview (carve.photos)_3` |

### Где найти спрайты:

- Откройте `Assets/Images/BasicArkanoidPack.png` в проекте
- Разверните файл — увидите отдельные под-спрайты (BasicArkanoidPack_0, BasicArkanoidPack_10, и т.д.)
- Перетаскивайте нужный под-спрайт на поле `sprite`

## 3. Добавить SkinApplier на префабы Platform и Ball

### Platform.prefab:

1. Откройте `Assets/Prefabs/Platform.prefab`
2. Добавьте компонент `SkinApplier` (Add Component → SkinApplier)
3. В поле **Skin Type** установите: `Platform`

### Ball.prefab:

1. Откройте `Assets/Prefabs/Ball.prefab`
2. Добавьте компонент `SkinApplier`
3. В поле **Skin Type** установите: `Ball`

### Ball Clone.prefab:

1. Откройте `Assets/Prefabs/Ball Clone.prefab`
2. Добавьте компонент `SkinApplier`
3. В поле **Skin Type** установите: `Ball`

## 4. Добавить новые скины

### Шаг 1: Добавить спрайт

1. Поместите PNG-файл скина в папку проекта
2. Unity автоматически импортирует файл как Sprite

### Шаг 2: Добавить запись в SkinManager

1. На сцене выберите GameObject `SkinManager`
2. В массиве `Skin Sprites` добавьте новый элемент:
   - `spriteName` = имя файла спрайта (без расширения)
   - `sprite` = перетащите спрайт
3. В массиве `Skin Name Aliases` добавьте новый элемент:
   - `dbSkinName` = имя скина в БД (поле `Name`)
   - `spriteKey` = значение из `spriteName` выше

## 5. Проверить работу

1. Запустите игру в Unity Editor
2. Откройте магазин из главного меню
3. Купите скин (если ещё не куплен)
4. Нажмите "Экипировать" — кнопка изменится на "ЭКИПИРОВАНО"
5. Закройте магазин и начните игру
6. Платформа/мяч должны использовать новый спрайт

## 6. Troubleshooting

- **Скин не применяется:** Проверьте консоль Unity. Убедитесь, что `spriteName` в Skin Sprites совпадает с `PrefabPath` из БД.
- **Sprite not found:** Проверьте, что `skinNameAliases` правильно настроен: `dbSkinName` = Name из БД, `spriteKey` = spriteName из Skin Sprites.
- **SkinApplier не работает:** Убедитесь, что компонент добавлен на префаб и `Skin Type` установлен правильно.
- **Кнопка "Экипировать" не появляется:** Проверьте, что `equipButton` привязан в `SkinItemUI` префаба.

# RustyProject API Setup

## Что реализовано
- Отдельный backend `UnityProjects/RustyAPI`
- JWT-авторизация
- Сохранение монет
- Сохранение прогресса по уровням и звездам
- Лидерборд по монетам
- SQL-скрипт с тестовыми данными: `UnityProjects/RustyAPI/sql/seed_test_data.sql`
- Форма входа/регистрации и лидерборд в главном меню создаются runtime-скриптом
- Монеты выводятся текстом в главном меню

## 1. Подготовить PostgreSQL
1. Убедиться, что PostgreSQL запущен на `localhost:5432`.
2. Создать базу данных:

```sql
CREATE DATABASE rusty_game;
```

3. При необходимости поменять логин, пароль или имя БД в:
- `UnityProjects/RustyAPI/appsettings.json`
- `UnityProjects/RustyAPI/appsettings.Development.json`
- `UnityProjects/RustyAPI/Database/RustyDbContextFactory.cs`

## 2. Запустить RustyAPI
Из папки `UnityProjects/RustyAPI` выполнить:

```powershell
dotnet build
dotnet run
```

API поднимется на:
- `http://localhost:5268`
- `https://localhost:7068`

Swagger будет доступен в development-режиме автоматически.

## 3. Применить тестовые данные
После первого запуска API создаст таблицы через миграции.

Затем выполнить SQL-скрипт:

```powershell
psql -h localhost -U postgres -d rusty_game -f "c:/directory-git/ktits-repository-4/UnityProjects/RustyAPI/sql/seed_test_data.sql"
```

Тестовые пользователи:
- `rusty_alex / password123`
- `rusty_nina / password123`
- `rusty_igor / password123`
- `rusty_lena / password123`
- `rusty_guest / password123`

## 4. Настроить Unity-клиент
Базовый URL клиента задается в:

- `UnityProjects/RustyProject/Assets/Scripts/Network/PlayerAccountManager.cs`

Если API запущен на другом адресе, поменять константу:

```csharp
private const string BaseApiUrl = "http://localhost:5268/api";
```

## 5. Что уже подключено в проекте
- `MainMenuUI.OpenAccount()` открывает runtime-форму входа/регистрации
- `MainMenuUI.OpenLeaderboard()` открывает runtime-лидерборд
- В форме аккаунта есть checkbox `Регистрация?`
- Монеты показываются в главном меню автоматически
- `GameManager` отправляет монеты на API при подборе монет
- `LevelManager` синхронизирует прогресс уровней и звезд с API
- `LevelSelectUI` обновляет доступность уровней после загрузки серверного профиля

## 6. Что проверить вручную
1. Запустить `RustyAPI`.
2. Открыть `RustyProject` в Unity.
3. На сцене меню нажать кнопку аккаунта и:
   - зарегистрировать нового пользователя, либо
   - войти под тестовым пользователем.
4. Проверить, что справа сверху появились имя пользователя и монеты.
5. Открыть лидерборд и убедиться, что в нем есть минимум 5 пользователей.
6. Запустить игру, собрать монеты и проверить, что после возврата в меню их значение обновилось.
7. Собрать звезды на уровне и проверить, что прогресс уровней сохраняется после возврата в меню и повторного входа.

## 7. Если кнопки в сцене еще не подвязаны
Если кнопки `Account` и `Leaderboard` в `SystemUIs` еще не привязаны в Inspector, привязать их к методам:
- `MainMenuUI.OpenAccount`
- `MainMenuUI.OpenLeaderboard`

Сами панели дополнительно создавать в сцене не нужно: они строятся из кода.

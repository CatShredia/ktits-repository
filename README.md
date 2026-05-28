# ИС производства конвейеров

Клиент-серверная информационная система для учёта заказов на производство конвейерного оборудования, складских запасов, спецификаций изделий, планировки цехов и производственного планирования.

Реализация выполнена по заданиям WSR (WorldSkills Russia) **DE09**, сессии 1–3. Подробное описание прав по ролям — в [role_functionality.md](role_functionality.md).

## Состав решения

| Проект | Назначение |
|--------|------------|
| `ProductionSystem.Api` | REST API (ASP.NET Core 10), JWT, Swagger |
| `ProductionSystem.Client` | Десктоп-клиент (Avalonia 11, .NET 8, MVVM) |
| `ProductionSystem.Data` | Сущности EF Core, миграции PostgreSQL |
| `ProductionSystem.Import` | Импорт справочников из Excel/CSV |

## Технологии

- **C#**, .NET 8 / .NET 10  
- **PostgreSQL** + Entity Framework Core  
- **Avalonia UI** (клиент), **CommunityToolkit.Mvvm**  
- **JWT** для авторизации API  

## Роли пользователей

| Роль | Основные возможности |
|------|----------------------|
| Заказчик | Регистрация, заказы (создание, просмотр, отмена) |
| Менеджер | Заказы, материалы и комплектующие, оценка закупки, диаграмма Ганта, отчёт по остаткам |
| Конструктор | Просмотр материалов и комплектующих |
| Мастер | Спецификации изделий, сбои оборудования, контроль качества |
| Директор | Просмотр заказов, планировка цехов, работники, отчёт по остаткам |

Учётные записи сотрудников загружаются утилитой импорта; заказчик регистрируется в клиенте.

## Функциональность

### Сессия 1
- Авторизация (JWT), регистрация заказчика  
- Склады, материалы, комплектующие  
- Справочник работников и производственных операций (директор)  

### Сессия 2
- Жизненный цикл заказов (статусы, история, фильтры)  
- Автосписание материалов при переходе «Закупка» → «Производство»  
- Планировка цехов (размещение значков на плане)  
- Сбои оборудования, контроль качества (ОТК)  

### Сессия 3
- Спецификации изделий (материалы, сборка, операции, чертежи, замеры)  
- Оценка закупки и минимальное время производства  
- Диаграмма Ганта по оборудованию  
- Отчёт по остаткам на складах (группировка по складам и типам)  

## Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (клиент)  
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (API, Import, Data)  
- [PostgreSQL](https://www.postgresql.org/)  
- `psql` в PATH (для скрипта настройки БД)  
- Windows (клиент Avalonia; API кроссплатформенный)  

## Быстрый старт

### 1. Настройка базы данных

Укажите строку подключения в `src/ProductionSystem.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "Default": "Host=localhost;Port=5432;Database=production_system;Username=postgres;Password=ВАШ_ПАРОЛЬ"
}
```

Из корня репозитория:

```powershell
.\scripts\setup-database.ps1
```

Скрипт применяет миграции EF Core, импортирует данные из `task/` и выполняет `scripts/seed-erd-test-data.sql`.

### 2. Запуск API

```powershell
dotnet run --project src/ProductionSystem.Api
```

По умолчанию: `http://localhost:5036`, Swagger: `/swagger`.

При старте API автоматически вызывается `MigrateAsync` и загрузка планов цехов.

### 3. Запуск клиента

В `src/ProductionSystem.Client/appsettings.json` должен совпадать адрес API:

```json
{
  "ApiBaseUrl": "http://localhost:5036"
}
```

```powershell
dotnet run --project src/ProductionSystem.Client -c Debug
```

Главное окно открывается сразу; вход — в отдельном окне поверх. Для заказчика после входа по умолчанию открывается раздел **«Заказы»**.

## Структура репозитория

```
task/                    # Материалы WSR (PDF, Excel, планы цехов, testdata)
scripts/
  setup-database.ps1     # Миграции + импорт + seed
  seed-erd-test-data.sql   # Тестовые заказы, цеха, спецификации
docs/                    # Задание и пояснительная записка (курсовой проект)
role_functionality.md    # Матрица прав по ролям
tests/
  ProductionSystem.Api.Tests/   # xUnit: API (интеграция) + сервисы
src/
  ProductionSystem.Api/
  ProductionSystem.Client/
  ProductionSystem.Data/
  ProductionSystem.Import/
```

## Полезные команды

```powershell
# Тесты API (xUnit, InMemory БД, Assert)
dotnet test tests/ProductionSystem.Api.Tests/ProductionSystem.Api.Tests.csproj
```

```powershell
# Миграции вручную
dotnet ef database update --project src/ProductionSystem.Data --startup-project src/ProductionSystem.Api

# Только импорт из Excel (с перезаписью)
dotnet run --project src/ProductionSystem.Import -- --force

# Сборка всего решения
dotnet build ProductionSystem.slnx
```

## Документация

- [role_functionality.md](role_functionality.md) — роли, меню, API  
- [docs/Задание_на_курсовой_проект.docx](docs/Задание_на_курсовой_проект.docx) — задание на КП  
- [docs/Курсовой_проект.docx](docs/Курсовой_проект.docx) — пояснительная записка  

Перегенерация docx из шаблона Runews:

```powershell
python docs/generate_course_docs.py
```

## Ограничения

- Пароли пользователей в БД хранятся в открытом виде (учебный проект).  
- Домашние экраны ролей — заглушки (только заголовок).  
- Для корректной работы клиента API должен быть запущен до входа.

## Логи клиента

При сбоях: `%AppData%\ProductionSystem.Client\crash.log`

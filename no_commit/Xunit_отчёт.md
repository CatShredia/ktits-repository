# Отчёт: разработка тестовых модулей (xUnit)

## Цель
Освоить модульное тестирование: создать отдельный проект xUnit и проверить бизнес-логику (`ProductService`) и работу с PostgreSQL (`ProductRepository`).

## Структура
```
ProductManagement.Solution/
├── ProductManagement/            # основной проект (net10.0, Npgsql)
│   ├── Product.cs
│   ├── ProductService.cs         # CalculateTotalPrice, CalculateDiscount, ValidateProduct
│   └── ProductRepository.cs      # Add, GetById, GetAll, Update, Delete
└── ProductManagement.Tests/      # xUnit, ссылка на основной проект
    ├── Services/ProductServiceTests.cs
    └── Repositories/ProductRepositoryTests.cs
```
Тестовая БД: Docker-контейнер `products-test-db` (postgres:17, порт 54320, база `products_test`). Строка подключения лежит в переменной окружения `PRODUCTS_TEST_DB`. Таблица создаётся в `EnsureCreated()`, до и после каждого теста очищается (`TRUNCATE`).

## Тесты (всего 34)
**ProductServiceTests (юнит)**
- `CalculateTotalPrice_ReturnsPriceTimesQuantity` [Theory]: 100×2=200, 250×4=1000, 50×0=0, 125,5×2=251.
- `CalculateTotalPrice_NegativePrice_Throws`, `..._NegativeQuantity_Throws`.
- `CalculateDiscount_ReturnsPriceWithDiscount` [Theory]: 1000/10%=900, 500/20%=400, 100/0%=100, 2000/50%=1000, 100/100%=0.
- `CalculateDiscount_InvalidPercent_Throws` [Theory]: -1, -0,01, 100,01, 150. `CalculateDiscount_NegativeTotal_Throws`.
- `ValidateProduct_*`: корректный товар (true); нулевое количество (true); пустое имя, нулевая цена, отрицательная цена, отрицательное количество, пустая категория (false); null (исключение).

**ProductRepositoryTests (интеграционные, PostgreSQL)**
Добавление, получение по Id, получение несуществующего (null), изменение, удаление, получение списка, Update/Delete несуществующей записи (false).

## Результаты
`dotnet test`: Passed 34, Failed 0.

## Выявленные и исправленные ошибки
В `CalculateTotalPrice` формула намеренно заменена с `price * quantity` на `price + quantity`.
Все 4 случая `CalculateTotalPrice_ReturnsPriceTimesQuantity` упали, например:
```
Assert.Equal() Failure: Values differ
Expected: 200
Actual:   102
```
Формула исправлена, повторный запуск: 34/34 успешно (логи: `broken_run.txt`, `fixed_run.txt`).

## Вывод
Модульные тесты быстро выявляют регрессии в расчётах и валидации. Интеграционные тесты подтверждают корректную работу CRUD с реальной PostgreSQL. Параметризованные тесты ([Theory]) покрывают таблицы значений из задания без дублирования кода.

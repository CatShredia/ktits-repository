# ApiSolution - Сценарии запуска

## 🚀 Быстрый старт

### Способ 1: PowerShell скрипт (рекомендуется)

```powershell
.\start-project.ps1
```

**Что делает:**
1. Собирает решение
2. Запускает API (порт 5039)
3. Запускает Blazor (порт 5156)
4. Открывает 2 вкладки в Chrome:
   - Swagger API
   - Blazor приложение

---

### Способ 2: Задачи VS Code

1. Откройте палитру команд: `Ctrl+Shift+P`
2. Выберите: **Tasks: Run Task**
3. Выберите: **Start All (API + Blazor)**

Затем вручную откройте:
- **API Swagger:** http://localhost:5039/swagger
- **Blazor App:** http://localhost:5156

---

### Способ 3: Отладка

1. **Debug API** — отладка API проекта
   - Выберите конфигурацию "Debug API"
   - Нажмите `F5`
   - Откроется Swagger на http://localhost:5039/swagger

2. **Debug Blazor WebAssembly** — отладка Blazor
   - Выберите конфигурацию "Debug Blazor WebAssembly"
   - Нажмите `F5`
   - Откроется браузер с Blazor приложением

3. **Debug All** — запуск обоих проектов
   - Выберите конфигурацию "Debug All (API + Blazor)"
   - Нажмите `F5`

---

## 📁 Структура конфигураций

| Файл | Назначение |
|------|------------|
| `.vscode/tasks.json` | Задачи для запуска проектов |
| `.vscode/launch.json` | Конфигурации отладки |
| `start-project.ps1` | PowerShell скрипт для быстрого старта |

---

## 🔗 Эндпоинты

| Сервис | URL | Описание |
|--------|-----|----------|
| API Swagger | http://localhost:5039/swagger | Документация API |
| Blazor App | http://localhost:5156 | Клиентское приложение |

---

## 🛑 Остановка

Закройте процессы `dotnet` в диспетчере задач или нажмите `Ctrl+C` в терминале.

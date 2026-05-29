# PracticalWork — Library & Reports

Два микросервиса на **.NET 10** с общей инфраструктурой (PostgreSQL, Redis, MinIO, RabbitMQ).


| Сервис                    | Назначение                                                                   |
| ------------------------- | ---------------------------------------------------------------------------- |
| **PracticalWork.Library** | Книги, читатели, выдача/возврат, фоновые задачи (Quartz), события в RabbitMQ |
| **PracticalWork.Reports** | Приём событий, логи активности, генерация CSV-отчётов, API отчётов           |


## Быстрый старт

```bash
# 1. Инфраструктура
docker compose up -d

# 2. Миграции
dotnet ef database update --project src/PracticalWork.Library.Data.PostgreSql
dotnet ef database update --project src/PracticalWork.Reports.Data.PostgreSql

# 3. Запуск API (два терминала)
dotnet run --project src/PracticalWork.Library.Web
dotnet run --project src/PracticalWork.Reports.Web
```


| Сервис  | Swagger                                                        |
| ------- | -------------------------------------------------------------- |
| Library | [http://localhost:5251/swagger](http://localhost:5251/swagger) |
| Reports | [http://localhost:5252/swagger](http://localhost:5252/swagger) |


Подробная инструкция: **[docs/DEPLOYMENT.md](docs/DEPLOYMENT.md)**

## Документация


| Тип              | Описание                                         | Ссылка                                                       |
| ---------------- | ------------------------------------------------ | ------------------------------------------------------------ |
| XML-комментарии  | `///` на public API, `GenerateDocumentationFile` | [docs/DOCUMENTATION.md](docs/DOCUMENTATION.md)               |
| Swagger          | OpenAPI + описания из XML                        | см. URL выше                                                 |
| README           | Этот файл                                        | —                                                            |
| Развёртывание    | Docker, БД, порты, troubleshooting               | [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md)                     |
| Рефакторинг      | Clean Architecture, чеклист                      | [REPORT.md](REPORT.md)                                       |
| Покрытие тестами | Coverlet, HTML-отчёт                             | [\coverage-report\index.html](..\coverage-report\index.html) |


## PracticalWork.Library

**Слои:** Controllers → Services → Repositories (PostgreSQL, Redis, MinIO, RabbitMQ).

**Основные API (v1):**


| Метод | Путь                         | Описание          |
| ----- | ---------------------------- | ----------------- |
| POST  | `/api/v1/books`              | Создать книгу     |
| PUT   | `/api/v1/books/{id}`         | Обновить книгу    |
| POST  | `/api/v1/books/{id}/archive` | Архивировать      |
| GET   | `/api/v1/books`              | Список с фильтром |
| POST  | `/api/v1/library/borrow`     | Выдача            |
| POST  | `/api/v1/library/return`     | Возврат           |
| GET   | `/api/v1/library/books`      | Доступные книги   |
| CRUD  | `/api/v1/readers`            | Читатели          |


**Фоновые задачи:** напоминание о возврате, еженедельный отчёт, архивация книг.

## PracticalWork.Reports

**Слои:** Web → Services → Repositories; consumer RabbitMQ в фоне.


| Метод | Путь                           | Описание                      |
| ----- | ------------------------------ | ----------------------------- |
| GET   | `/api/reports/activity`        | Логи активности (пагинация)   |
| POST  | `/api/reports/generate`        | Сгенерировать отчёт за период |
| GET   | `/api/reports`                 | Список отчётов                |
| GET   | `/api/reports/{name}/download` | Presigned URL                 |


## Инфраструктура

```
docker compose up -d
```


| Компонент          | UI / порт                                                                    |
| ------------------ | ---------------------------------------------------------------------------- |
| MinIO              | [http://localhost:9001](http://localhost:9001) (`minioadmin` / `minioadmin`) |
| RabbitMQ           | [http://localhost:15672](http://localhost:15672) (`rabbit` / `rabbit`)       |
| smtp4dev           | [http://localhost:3000](http://localhost:3000)                               |
| PostgreSQL Library | `localhost:5440`                                                             |
| PostgreSQL Reports | `localhost:5433`                                                             |
| Redis              | `localhost:6379`                                                             |


## Тесты

```bash
dotnet test PracticalWork.Library.sln
```

Отчёт покрытия (67+ тестов):

```cmd
scripts\Generate-CoverageReport.cmd
```

→ `coverage-report\index.html`

## Структура решения

```
src/
  PracticalWork.Library.*          # Домен и API библиотеки
  PracticalWork.Reports.*          # Отчёты и логи
tests/
  PracticalWork.Library.Tests
  PracticalWork.Reports.Tests
utils/
  PracticalWork.Library.Data.PostgreSql.Migrator
docs/                              # DEPLOYMENT, DOCUMENTATION
```

## Сборка

```bash
dotnet build PracticalWork.Library.sln
```

Требуется **.NET 10 SDK**.
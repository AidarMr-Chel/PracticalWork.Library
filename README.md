PracticalWork — Library & Reports Microservices

Общие сведения
Проект состоит из двух независимых микросервисов:
1. PracticalWork.Library — сервис управления библиотекой
2. PracticalWork.Reports — сервис генерации отчётов и логирования активности

Оба сервиса используют общую инфраструктуру: PostgreSQL, Redis, MinIO, RabbitMQ, Docker Compose.

============================================================
PracticalWork.Library
============================================================

Назначение:
- Управление книгами и читателями
- Выдача и возврат книг
- Публикация событий в RabbitMQ
- Хранение обложек книг в MinIO
- Кэширование списков и деталей в Redis

Исполняемые модули:
- PracticalWork.Library.Web — ASP.NET 8 WebApi
- PracticalWork.Library.Data.PostgreSql.Migrator — миграции БД

Интеграции:
- PostgreSQL — основная база данных
- Redis — кэширование
- MinIO — хранение файлов
- RabbitMQ — публикация событий

============================================================
PracticalWork.Reports
============================================================

Назначение:
- Подписка на события из сервиса библиотеки
- Запись активности в PostgreSQL
- Генерация CSV‑отчётов
- Хранение отчётов в MinIO
- Кэширование списка отчётов в Redis
- API для логов, отчётов и скачивания файлов

Исполняемые модули:
- PracticalWork.Reports.Web — ASP.NET 8 WebApi
- PracticalWork.Reports.Data.PostgreSql.Migrator — миграции БД
- PracticalWork.Reports.MessageBroker.Consumer — подписка на RabbitMQ

Интеграции:
- PostgreSQL — база данных отчётов и логов
- Redis — кэш списка отчётов
- MinIO — хранилище CSV‑файлов
- RabbitMQ — получение событий

============================================================
Инструкция по запуску
============================================================

Предварительные требования:
- Docker + Docker Compose
- .NET 8 SDK
- PostgreSQL клиент (pgAdmin / DBeaver)
- Redis CLI (опционально)
- RabbitMQ Management Console

Запуск инфраструктуры:
 docker-compose up -d

Применение миграций:
 Library:
  dotnet ef database update --project src/PracticalWork.Library.Data.PostgreSql
 Reports:
  dotnet ef database update --project src/PracticalWork.Reports.Data.PostgreSql

Запуск сервисов:
 Library:
  dotnet run --project src/PracticalWork.Library.Web
 Reports:
  dotnet run --project src/PracticalWork.Reports.Web

Swagger:
 Library: http://localhost:5251/swagger
 Reports: http://localhost:5252/swagger

============================================================
Основные сценарии Library
============================================================

- POST /api/v1/books — добавление книги
- PUT /api/v1/books/{id} — редактирование книги
- POST /api/v1/books/{id}/archive — архивирование книги
- POST /api/v1/library/borrow — выдача книги
- POST /api/v1/library/return — возврат книги
- GET /api/v1/library/books — доступные книги

============================================================
Основные сценарии Reports
============================================================

- GET /api/reports/activity — получение логов активности
- POST /api/reports/generate — генерация отчёта
- GET /api/reports — список отчётов
- GET /api/reports/{reportName}/download — скачивание отчёта

============================================================
Проверка инфраструктуры
============================================================

Redis:
 redis-cli keys *

MinIO:
 http://localhost:9001 (minioadmin/minioadmin)
 Бакет: reports

RabbitMQ:
 http://localhost:15672 (guest/guest)
 Очередь: reports.activity

============================================================
Архитектурные решения
============================================================

Library:
- Чистая архитектура
- Контроллеры → Сервисы → Репозитории
- Кэширование Redis
- Хранение файлов MinIO
- Публикация событий RabbitMQ
- Инвалидация кэша через реестр ключей

Reports:
- Подписка на события RabbitMQ
- Запись логов в PostgreSQL
- Генерация CSV
- Хранение файлов в MinIO
- Кэширование списка отчётов
- API для логов и отчётов
- Signed URL для скачивания

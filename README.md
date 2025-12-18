PracticalWork — Library & Reports Microservices

Общие сведения
Проект состоит из двух независимых микросервисов:
1. PracticalWork.Library — сервис управления библиотекой
2. PracticalWork.Reports — сервис генерации отчётов и логирования активности

Оба сервиса используют общую инфраструктуру: PostgreSQL, Redis, MinIO, RabbitMQ, Docker Compose.

PracticalWork.Library
Назначение: управление библиотекой, публикация событий.
Интеграции: PostgreSQL, Redis, MinIO, RabbitMQ.

PracticalWork.Reports
Назначение: подписка на события, запись активности, генерация CSV отчётов, хранение в MinIO, кэширование Redis.
Интеграции: PostgreSQL, Redis, MinIO, RabbitMQ.

Запуск инфраструктуры:
 docker-compose up -d

Миграции:
 dotnet ef database update --project src/PracticalWork.Library.Data.PostgreSql
 dotnet ef database update --project src/PracticalWork.Reports.Data.PostgreSql

Запуск сервисов:
 dotnet run --project src/PracticalWork.Library.Web
 dotnet run --project src/PracticalWork.Reports.Web

Swagger:
 Library: http://localhost:5251/swagger
 Reports: http://localhost:5252/swagger

Основные сценарии Library:
 - POST /api/v1/books
 - POST /api/v1/books/{id}/archive
 - POST /api/v1/library/borrow
 - POST /api/v1/library/return

Основные сценарии Reports:
 - GET /api/reports/activity
 - POST /api/reports/generate
 - GET /api/reports
 - GET /api/reports/{reportName}/download

Проверка Redis:
 redis-cli keys *

Проверка MinIO:
 http://localhost:9001 (minioadmin/minioadmin)

Проверка RabbitMQ:
 http://localhost:15672 (guest/guest)

Архитектурные решения:
 - Чистая архитектура
 - Разделение слоёв
 - Кэширование
 - Хранение файлов
 - Подписка на события

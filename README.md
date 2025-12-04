Общие сведения

Наименование сервиса

PracticalWork.Library

Назначение

-   Получение опыта в основах ООП и Docker.
-   Разработка системы управления библиотекой.

Исполняемые модули

1.  PracticalWork.Library.Web — ASP.NET 8 WebApi
2.  PracticalWork.Library.Data.PostgreSql.Migrator — запуск миграций

Интеграции

1.  База данных — PostgreSQL
2.  Распределенный кэш — Redis
3.  Хранение файлов — MinIO

Инструменты разработки

1.  Rider, Visual Studio 2022 или VS Code
2.  PostgreSQL pgAdmin или DBeaver
3.  Redis Insight (опционально)

------------------------------------------------------------------------

Инструкция по запуску и тестированию

Предварительные требования

-   Установлен Docker и Docker Compose
-   Установлен .NET 8 SDK (или версия, указанная в проекте)
-   Установлен PostgreSQL клиент (для проверки БД вручную)
-   Установлен Redis CLI (опционально, для проверки кэша)

Запуск проекта

Собери и запусти инфраструктуру (Postgres, Redis, MinIO):

    docker-compose up -d

Это поднимет три контейнера: - library_postgres — база данных
PostgreSQL - library_redis — кэш Redis - minio — файловое хранилище для
обложек книг

Примените миграции к базе данных:

    dotnet ef database update --project src/PracticalWork.Library.Data.PostgreSql

Запусти веб-приложение:

    dotnet run --project src/PracticalWork.Library.Web

Приложение будет доступно по адресу: http://localhost:5251

------------------------------------------------------------------------

Тестирование API

Swagger доступен по адресу: http://localhost:5251/swagger

Основные сценарии

-   Добавление книги — POST /api/v1/books
-   Редактирование книги — PUT /api/v1/books/{id}
-   Архивирование книги — POST /api/v1/books/{id}/archive
-   Получение списка книг — GET /api/v1/books?PageNumber=1&PageSize=10
-   Добавление деталей книги (обложка) — POST /api/v1/books/{id}/details
-   Выдача книги читателю —
    POST /api/v1/library/borrow?bookId={id}&readerId={id}
-   Возврат книги — POST /api/v1/library/return?bookId={id}
-   Получение доступных книг — GET /api/v1/library/books
-   Детали выдачи книги — GET /api/v1/library/{idOrReader}/details

------------------------------------------------------------------------

Проверка кэша Redis

    docker exec -it library_redis redis-cli
    keys *

------------------------------------------------------------------------

Проверка MinIO

Консоль MinIO: http://localhost:9001 Логин/пароль:
minioadmin / minioadmin Загруженные обложки хранятся в бакете
book-covers.

------------------------------------------------------------------------

Примеры запросов для проверки функциональности

Добавление книги

    curl -X POST http://localhost:5251/api/v1/books \
      -H "Content-Type: application/json" \
      -d '{
        "title": "Преступление и наказание",
        "category": 1,
        "authors": ["Ф.М. Достоевский"],
        "year": 1866,
        "description": "Роман о судьбе студента Раскольникова"
      }'

Редактирование книги

    curl -X PUT http://localhost:5251/api/v1/books/{bookId} \
      -H "Content-Type: application/json" \
      -d '{
        "title": "Преступление и наказание (ред.)",
        "authors": ["Ф.М. Достоевский"],
        "year": 1866,
        "description": "Обновлённое описание"
      }'

Архивирование книги

    curl -X POST http://localhost:5251/api/v1/books/{bookId}/archive

Получение списка книг

    curl "http://localhost:5251/api/v1/books?PageNumber=1&PageSize=10&Category=1&Status=Available"

Добавление деталей книги (обложка)

    curl -X POST http://localhost:5251/api/v1/books/{bookId}/details \
      -F "description=Новая обложка" \
      -F "coverFile=@cover.png"

Выдача книги читателю

    curl -X POST "http://localhost:5251/api/v1/library/borrow?bookId={bookId}&readerId={readerId}"

Возврат книги

    curl -X POST "http://localhost:5251/api/v1/library/return?bookId={bookId}"

Получение доступных книг

    curl "http://localhost:5251/api/v1/library/books"

Детали выдачи книги

    curl "http://localhost:5251/api/v1/library/{borrowId}/details"

------------------------------------------------------------------------

Описание архитектурных решений

1. Слой Contracts

DTO-объекты (BookFilterRequest, CreateBookRequest,
BorrowDetailsResponse) для обмена данными между API и клиентами.
Используются nullable-поля для гибкой фильтрации.

2. Слой Controllers

ASP.NET Core контроллеры (BooksController, BorrowController) принимают
запросы. Валидация входных данных через ModelState. Swagger подключён
для документирования API.

3. Слой Services

BookService

-   CRUD-операции над книгами
-   Интеграция с Redis для кэширования списков и деталей
-   Интеграция с MinIO для хранения обложек
-   Инвалидация кэша при изменениях

BorrowService

-   Логика выдачи и возврата книг
-   Проверка статуса книги и активности читателя
-   Кэширование доступных книг и деталей выдачи
-   Инвалидация кэша при изменениях

4. Слой Repositories

Реализация доступа к PostgreSQL через EF Core. Репозитории
(BookRepository, BorrowRepository, ReaderRepository) отвечают только за
CRUD. Бизнес-логика вынесена в сервисы.

5. Инфраструктура

-   PostgreSQL — основная база данных
-   Redis — распределённый кэш для ускорения запросов
-   MinIO — объектное хранилище для файлов (обложки книг)
-   Все сервисы запускаются через docker-compose

6. Архитектурные принципы

-   Чистая архитектура: разделение слоёв (Contracts -> Controllers ->
    Services -> Repositories -> Storage)
-   Инвалидация кэша через реестр ключей
-   Loose coupling: работа через интерфейсы
-   API Versioning: используется Asp.Versioning для поддержки нескольких
    версий API

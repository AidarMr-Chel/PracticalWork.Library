# \# Общие сведения

# 

# \### Наименование сервиса

# 

# PracticalWork.Library

# 

# \### Назначение

# 

# \-   Получение опыта в основах ООП и Docker.

# \-   Разработка системы управления библиотекой.

# 

# \### Исполняемые модули

# 

# 1\.  \*\*PracticalWork.Library.Web\*\* --- ASP.NET 8 WebApi

# 2\.  \*\*PracticalWork.Library.Data.PostgreSql.Migrator\*\* --- запуск

# &nbsp;   миграций

# 

# ------------------------------------------------------------------------

# 

# \# Инструкция по запуску и тестированию

# 

# \## Предварительные требования

# 

# \-   Установлен Docker и Docker Compose

# \-   Установлен .NET 8 SDK

# \-   Установлен PostgreSQL клиент

# \-   Установлен Redis CLI (опционально)

# 

# \## Запуск проекта

# 

# ``` bash

# docker-compose up -d

# ```

# 

# ``` bash

# dotnet ef database update --project src/PracticalWork.Library.Data.PostgreSql

# ```

# 

# ``` bash

# dotnet run --project src/PracticalWork.Library.Web

# ```

# 

# Swagger:\\

# http://localhost:5251/swagger

# 

# ------------------------------------------------------------------------

# 

# \# Архитектурные решения

# 

# \## 1. Слой Contracts

# 

# DTO-объекты для обмена между API и клиентами. Используются nullable‑поля

# для фильтрации.

# 

# \## 2. Слой Controllers

# 

# Контроллеры принимают запросы, валидация через `ModelState`,

# документация через Swagger.

# 

# \## 3. Слой Services

# 

# \### BookService

# 

# \-   CRUD операции

# \-   Кэш Redis

# \-   Хранение обложек MinIO

# \-   Инвалидация кэша

# 

# \### BorrowService

# 

# \-   Логика выдачи/возврата

# \-   Проверка статусов

# \-   Кэширование

# \-   Инвалидация

# 

# \## 4. Слой Repositories

# 

# Доступ к PostgreSQL через EF Core. Репозитории только для CRUD, логика в

# сервисах.

# 

# \## 5. Инфраструктура

# 

# \-   PostgreSQL --- база

# \-   Redis --- распределенный кэш

# \-   MinIO --- объектное хранилище

# \-   Запуск через docker-compose

# 

# \## 6. Принципы

# 

# \-   Чистая архитектура

# \-   Loose coupling через интерфейсы

# \-   API Versioning через Asp.Versioning




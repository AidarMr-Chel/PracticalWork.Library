# Развёртывание PracticalWork.Library и PracticalWork.Reports

Инструкция для локальной разработки и демонстрации. Оба сервиса используют общую инфраструктуру из `docker-compose.yaml`.

## 1. Требования


| Компонент      | Версия     |
| -------------- | ---------- |
| .NET SDK       | 10.0+      |
| Docker Desktop | актуальная |
| Git            | любая      |


Опционально: pgAdmin/DBeaver, Redis CLI, браузер для Swagger.

## 2. Инфраструктура (Docker Compose)

Из корня репозитория:

```bash
docker compose up -d
```

Поднимаются:


| Сервис               | Порт         | Учётные данные                         |
| -------------------- | ------------ | -------------------------------------- |
| PostgreSQL (Library) | **5440**     | `postgres` / `135792468`, БД `library` |
| PostgreSQL (Reports) | **5433**     | `postgres` / `135792468`, БД `reports` |
| Redis                | 6379         | без пароля                             |
| MinIO API / Console  | 9000 / 9001  | `minioadmin` / `minioadmin`            |
| RabbitMQ AMQP / UI   | 5672 / 15672 | `rabbit` / `rabbit`                    |
| smtp4dev (SMTP + UI) | 2525 / 3000  | без авторизации                        |


Проверка:

```bash
docker compose ps
```

## 3. Миграции баз данных

### Library

```bash
dotnet ef database update --project src/PracticalWork.Library.Data.PostgreSql
```

Строка подключения по умолчанию: `Host=localhost;Port=5440;Database=library;...` (см. `src/PracticalWork.Library.Web/appsettings.json`).

### Reports

```bash
dotnet ef database update --project src/PracticalWork.Reports.Data.PostgreSql
```

Строка подключения: `Host=localhost;Port=5433;Database=reports;...` (см. `src/PracticalWork.Reports.Web/appsettings.json`).

> Альтернатива: утилита `utils/PracticalWork.Library.Data.PostgreSql.Migrator` для Library.

## 4. Запуск API

Терминал 1 — **Library**:

```bash
dotnet run --project src/PracticalWork.Library.Web
```

Терминал 2 — **Reports** (после Library, чтобы события RabbitMQ публиковались):

```bash
dotnet run --project src/PracticalWork.Reports.Web
```


| Сервис  | URL                                            | Swagger                                                        |
| ------- | ---------------------------------------------- | -------------------------------------------------------------- |
| Library | [http://localhost:5251](http://localhost:5251) | [http://localhost:5251/swagger](http://localhost:5251/swagger) |
| Reports | [http://localhost:5252](http://localhost:5252) | [http://localhost:5252/swagger](http://localhost:5252/swagger) |


## 5. MinIO — бакеты

После первого запуска создайте в [консоли MinIO](http://localhost:9001) бакеты (если не созданы автоматически):

- `book-covers` — обложки книг (Library)
- `reports` — CSV-отчёты (Reports)

## 6. RabbitMQ

- Exchange (Library): `library.events` (настраивается в `appsettings.json`)
- Очередь Reports: `reports.activity` (consumer в `PracticalWork.Reports.MessageBroker`)

Убедитесь, что **Reports** запущен до массовой публикации событий из Library.

## 7. Сборка и тесты

```bash
dotnet build PracticalWork.Library.sln
dotnet test PracticalWork.Library.sln
```

Покрытие кода: см. [\coverage-report\index.html](..\coverage-report\index.html).

## 8. Типичные проблемы


| Симптом                      | Решение                                                              |
| ---------------------------- | -------------------------------------------------------------------- |
| Нет подключения к PostgreSQL | Проверьте порты 5440 / 5433 и `docker compose ps`                    |
| Reports не видит события     | Запустите Reports, проверьте RabbitMQ UI и exchange                  |
| SMTP не отправляет письма    | Используйте smtp4dev: [http://localhost:3000](http://localhost:3000) |
| Swagger без описаний         | Пересоберите Web-проект (`dotnet build`) — XML копируется в output   |


## 9. Переменные окружения

Для production переопределяйте секцию `App` и связанные настройки через:

- `appsettings.Production.json`
- переменные окружения ASP.NET Core (`App__DbConnectionString`, `App__Redis__Configuration`, …)

Не храните production-пароли в репозитории.
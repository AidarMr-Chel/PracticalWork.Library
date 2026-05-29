# Отчёт о рефакторинге PracticalWork.Library

---

## 1. Сводная таблица

| № | Критерий (чеклист) | До рефакторинга | После рефакторинга |
|---|-------------------|-----------------|---------------------|
| 1 | Прямые зависимости API от Infrastructure | Reports.Web тянул все инфраструктурные проекты; Library.Web — ожидаемо как composition root | Reports: DI вынесен в `AddReportsApplication()`; Library: `AddLibraryApplication()` |
| 2 | `DbContext` в контроллерах | `ActivityController` использовал `ReportsDbContext` | Исправлено: `IActivityLogService` |
| 3 | Отсутствие интерфейсов у репозиториев/сервисов | Reports: конкретные классы в DI | Исправлено: порты в `Reports.Entities.Abstractions`, сервисы за интерфейсами |
| 4 | Бизнес-логика в контроллерах | `ActivityController`, `BorrowController.GetDetails` | Исправлено: логика в сервисах |
| 5 | «Толстые» контроллеры | `ActivityController.Get` (~30 строк логики) | Исправлено: тонкие контроллеры |
| 6 | Смешение слоёв | `Library` → `MessageBroker`; MinIO SDK в доменном проекте | Частично: `IMessagePublisher` перенесён; пакет `Minio` убран из `Library.csproj` |
| 7 | Отсутствие валидации входных данных | Только для книг | Исправлено: Readers, Borrow, Reports DTO |
| 8 | Жёсткие зависимости (`new` вместо DI) | RabbitMQ в конструкторах consumer/publisher | Исправлено: `IRabbitMqConnection`, `IReportsRabbitMqChannel` |
| 9 | Нарушение SRP | `WeeklyReportWorkflow`, `ReportService` | Исправлено: разбиение weekly report на компоненты |
| 10 | Отсутствие асинхронности в I/O | В целом соблюдалось | Без изменений; пагинация перенесена в БД |
| 11 | Монолитная логика фоновых задач | `ReportsEventConsumer` с прямым `DbContext` | Исправлено: `IActivityLogIngestionService`, ack/nack |
| 12 | Прямые вызовы MinIO/брокера без интерфейсов | Reports: Redis без абстракции; MinIO в `BorrowController` | Исправлено: `IReportsCacheService`; MinIO в `BorrowService` |

---

## 2. Детализация по критериям

### 2.1. Прямые зависимости Infrastructure в API-слое

**Выявлено:**
- `PracticalWork.Library.Controllers` — без прямых ссылок на инфраструктуру (корректно).
- `PracticalWork.Library.Web` — ссылки на PostgreSQL, MinIO, Redis, RabbitMQ, Jobs (допустимо для composition root, но раздувает `Startup`).
- `PracticalWork.Reports.Web` — прямая регистрация DbContext, репозиториев и сервисов в `Program.cs`.

**Исправлено:**
- `src/PracticalWork.Library.Web/Infrastructure/ServiceCollectionExtensions.cs` — метод `AddLibraryApplication()`.
- `src/PracticalWork.Reports.Web/Infrastructure/ServiceCollectionExtensions.cs` — метод `AddReportsApplication()`.
- `Startup.cs` / `Program.cs` упрощены до вызова extension-методов.

---

### 2.2. Использование `DbContext` в контроллерах

**Выявлено:**
- `Reports.Web/Controllers/ActivityController` — инъекция `ReportsDbContext`, построение LINQ-запросов, пагинация и маппинг в DTO внутри контроллера.

**Исправлено:**
- `IActivityLogRepository.GetPagedAsync()` — доступ к данным.
- `IActivityLogService` / `ActivityLogService` — сценарий чтения.
- `ActivityController` — одна строка: делегирование в сервис.

**Файлы:**
- `Reports.Entities/Abstractions/IActivityLogRepository.cs`
- `Reports.Services/ActivityLogService.cs`
- `Reports.Web/Controllers/ActivityController.cs`

---

### 2.3. Отсутствие интерфейсов для репозиториев и сервисов

**Выявлено (Reports):**
- `ReportService`, `ActivityLogRepository`, `ReportRepository`, `RedisCacheService` регистрировались как конкретные типы.
- `ReportsController` зависел от `ReportService` без интерфейса.

**Исправлено:**
- Порты: `IActivityLogRepository`, `IReportRepository`, `IReportsCacheService` (`Reports.Entities.Abstractions`).
- Сервисы: `IActivityLogService`, `IReportService`, `IActivityLogIngestionService` (`Reports.Services.Abstractions`).
- Регистрация: `Reports.Services/Entry.cs`, `Reports.Data.PostgreSql/Entry.cs`, `Cache.Redis/Entry.cs`.

---

### 2.4. Бизнес-логика в контроллерах

**Выявлено:**
- `BorrowController.GetDetails` — получение URL обложки через `IMinioService`, сборка `BorrowDetailsResponse`.
- `ActivityController.Get` — фильтрация, подсчёт, пагинация, проекция.

**Исправлено:**
- `IBorrowService.GetBorrowDetailsAsync()` — MinIO и маппинг в `BorrowService`.
- `IActivityLogService.GetPagedAsync()` — вся логика чтения логов.

---

### 2.5. «Толстые» контроллеры (>10 строк логики в методе)

**Выявлено:**
- `ActivityController.Get` — ~30 строк.
- `BorrowController.GetDetails` — ~20 строк.

**Исправлено:** методы контроллеров сокращены до делегирования в сервисы (обычно 3–8 строк).

---

### 2.6. Смешение слоёв

**Выявлено:**
- `PracticalWork.Library` ссылался на `PracticalWork.Library.MessageBroker` (инфраструктура в домене).
- `IMessagePublisher` находился в сборке MessageBroker.
- Пакет NuGet `Minio` в `PracticalWork.Library.csproj` при использовании только `IMinioService`.

**Исправлено:**
- `IMessagePublisher` перенесён в `Library/Abstractions/Messaging/IMessagePublisher.cs`.
- Ссылка `Library` → `MessageBroker` удалена из `PracticalWork.Library.csproj`.
- Пакет `Minio` удалён из доменного проекта (реализация остаётся в `Data.Minio`).

---

### 2.7. Отсутствие валидации входных данных

**Выявлено:**
- FluentValidation только для `CreateBookRequest`, `UpdateBookRequest`, `UpdateBookDetailsRequest`.
- Нет валидаторов для Readers, Borrow (query), Reports DTO.

**Исправлено:**

| Валидатор | Файл |
|-----------|------|
| `CreateReaderRequestValidator` | `Library.Controllers/Validations/v1/` |
| `ExtendReaderRequestValidator` | то же |
| `CreateBorrowQueryValidator` | то же |
| `ReturnBookQueryValidator` | то же |
| `BookFilterRequestValidator` | то же |
| `GenerateReportRequestValidator` | `Reports.Web/Validations/` |
| `ActivityLogFilterDtoValidator` | то же |

Reports: подключены `AddValidatorsFromAssemblyContaining` и `AddFluentValidationAutoValidation` в `AddReportsApplication()`.

---

### 2.8. Жёсткие зависимости (`new` вместо DI)

**Выявлено:**
- `ReportsEventConsumer` — `new ConnectionFactory()`, `CreateConnection()` в конструкторе.
- `RabbitMqMessagePublisher` — аналогично.
- `SmtpEmailService` — `new SmtpClient()` в методе (допустимо для short-lived client).

**Исправлено:**

| Компонент | Решение |
|-----------|---------|
| Library publisher | `IRabbitMqConnection` / `RabbitMqConnection` (singleton) |
| Reports consumer | `IReportsRabbitMqChannel` / `ReportsRabbitMqChannel` (singleton) |
| Регистрация | `MessageBroker/Entry.cs`, `Reports.MessageBroker/Entry.cs` |

---

### 2.9. Нарушение Single Responsibility Principle (SRP)

**Выявлено:**
- `WeeklyReportWorkflow` (~270 строк) — период, агрегация, CSV, MinIO, email, алерты.
- `ReportService` — генерация, MinIO, БД, кэш, список файлов.
- `BookService` — CRUD, кэш, события, MinIO (приемлемо для application service, но перегружен).

**Исправлено (Weekly Report):**

| Компонент | Ответственность |
|-----------|----------------|
| `WeeklyReportPeriodProvider` | Расчёт диапазона дат |
| `WeeklyReportStatisticsAggregator` | Агрегация событий |
| `WeeklyReportCsvBuilder` | CSV и параметры шаблона |
| `WeeklyReportFilePublisher` | Загрузка в MinIO, presigned URL |
| `WeeklyReportEmailNotifier` | Отправка отчёта и алертов |
| `WeeklyReportWorkflow` | Оркестрация (~90 строк) |

Модель `WeeklyStats` вынесена в `Models/WeeklyStats.cs`.

---

### 2.10. Отсутствие асинхронности в I/O

**Выявлено:** критических синхронных блокировок I/O не обнаружено.

**Улучшено:**
- `BookRepository.FindPagedAsync()` — `Skip`/`Take` в SQL вместо пагинации в памяти в `BookService.GetBooks`.
- `BorrowService` — `excludeArchived: true` на уровне запроса, без `Where` в памяти.

---

### 2.11. Фоновые задачи с монолитной логикой

**Выявлено:**
- `ReportsEventConsumer` — парсинг сообщения, scope, `DbContext`, `SaveChanges` в одном обработчике; `autoAck: true` без обработки ошибок.

**Исправлено:**
- `IActivityLogIngestionService` / `ActivityLogIngestionService` — сохранение события.
- `autoAck: false`, `BasicAck` / `BasicNack` при успехе/ошибке.
- `AsyncEventingBasicConsumer` для асинхронной обработки.

**Library (Quartz):** изначально корректно — jobs делегируют в workflow (`ReturnReminderJob`, `ArchiveBooksJob`, `WeeklyReportJob`). Дублирующая регистрация `WeeklyReportWorkflow` в Jobs убрана; регистрация в `AddDomain()`.

---

### 2.12. Прямые вызовы внешних сервисов без интерфейсов

**Выявлено:**
- `BorrowController` — прямой `IMinioService`.
- `ReportService` — конкретный `RedisCacheService`.

**Исправлено:**
- MinIO для borrow — только через `BorrowService`.
- Reports cache — `IReportsCacheService` → `RedisCacheService`.
- MinIO в Reports — по-прежнему `IMinioService` (уже был интерфейс).

---

## 3. Дополнительные улучшения

| Улучшение | Описание |
|-----------|----------|
| Пагинация в БД | `IBookRepository.FindPagedAsync`, `ApplyFilter` в `BookRepository` |
| RabbitMQ DI | Singleton-подключения, отдельные Entry для Library и Reports |
| Composition root | `AddLibraryApplication()`, `AddReportsApplication()` |
| Удаление дублирования DI | `WeeklyReportWorkflow` регистрируется один раз в `AddDomain()` |
| Исправления сборки | XML-документация `RedisCacheService`, nullable в `BorrowService`, API `BasicConsume` для RabbitMQ.Client |

---

## 4. Затронутые проекты и ключевые файлы

### Новые файлы (основное)

- `Reports.Entities/Abstractions/*`
- `Reports.Services/Abstractions/*`, `ActivityLogService.cs`, `ActivityLogIngestionService.cs`, `Entry.cs`
- `Reports.Data.PostgreSql/Entry.cs`
- `Reports.MessageBroker/Entry.cs`, `IReportsRabbitMqChannel.cs`, `ReportsRabbitMqChannel.cs`
- `Reports.Web/Infrastructure/ServiceCollectionExtensions.cs`
- `Library/Abstractions/Messaging/IMessagePublisher.cs`
- `Library/Services/Reports/*` (weekly report components)
- `Library/Models/WeeklyStats.cs`
- `Library.Web/Infrastructure/ServiceCollectionExtensions.cs`
- `Library.MessageBroker/RabbitMQ/IRabbitMqConnection.cs`, `RabbitMqConnection.cs`
- `Library.Controllers/Validations/v1/*` (Readers, Borrow, BookFilter)
- `Reports.Web/Validations/*`

### Существенно изменённые

- `ActivityController.cs`, `ReportsController.cs`, `BorrowController.cs`
- `BookRepository.cs`, `IBookRepository.cs`, `BookService.cs`, `BorrowService.cs`
- `WeeklyReportWorkflow.cs`, `Entry.cs` (Library)
- `Startup.cs`, `Program.cs` (оба Web)
- `ReportsEventConsumer.cs`, `RabbitMqMessagePublisher.cs`
- `ReportService.cs`, `RedisCacheService.cs`

### Удалённые

- `Library.MessageBroker/Abstractions/IMessagePublisher.cs` (перенесён в Library.Abstractions)

---

## 5. Заключение

Рефакторинг затронул прежде всего **модуль отчётов**, где нарушения Clean Architecture были наиболее выражены (`DbContext` в API, отсутствие интерфейсов, логика в контроллерах). **Основной модуль библиотеки** изначально был ближе к целевой архитектуре; доработки коснулись borrow/MinIO, валидации, messaging, weekly report и пагинации.
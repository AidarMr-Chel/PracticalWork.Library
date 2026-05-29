# Документация проекта

Перечень артефактов документации по заданию «Документация проектов».

## 1. XML-документация кода

**Где включено:** `Directory.Build.props` — для всех проектов решения:

```xml
<GenerateDocumentationFile>true</GenerateDocumentationFile>
```

**Формат:** стандартные комментарии C# `///` на public-типах и членах.

**Результат сборки:** файлы `*.xml` рядом с `*.dll` в `bin/Debug/net10.0/` каждого проекта.

**Основные сборки с описаниями API:**

- `PracticalWork.Library.Controllers` — контроллеры Library
- `PracticalWork.Library.Contracts` — DTO запросов/ответов
- `PracticalWork.Library` — домен, сервисы, абстракции
- `PracticalWork.Reports.Web` — контроллеры Reports
- `PracticalWork.Reports.Entities` — DTO и сущности Reports

Предупреждение CS1591 (отсутствующий XML) подавлено глобально; для нового public API добавляйте `///`.

## 2. Swagger / OpenAPI


| Сервис  | Проект                      | Конфигурация                                                          |
| ------- | --------------------------- | --------------------------------------------------------------------- |
| Library | `PracticalWork.Library.Web` | `ServiceCollectionExtensions.AddLibraryApplication` → `AddSwaggerGen` |
| Reports | `PracticalWork.Reports.Web` | `Infrastructure/SwaggerExtensions.cs`                                 |


XML-комментарии подключаются из всех `*.xml` в каталоге выхода Web-приложения (см. `build/CopyReferencedXmlDocs.targets`).

**Эндпоинты Swagger UI:**

- Library: [http://localhost:5251/swagger](http://localhost:5251/swagger)  
- Reports: [http://localhost:5252/swagger](http://localhost:5252/swagger)

OpenAPI JSON:

- `/swagger/v1/swagger.json` (Reports)
- `/swagger/v1/swagger.json` (Library, версия API v1)

## 3. README

Корневой [README.md](../README.md) — обзор обоих сервисов, быстрый старт, ссылки на детальные инструкции.

## 4. Инструкция по развёртыванию

[docs/DEPLOYMENT.md](DEPLOYMENT.md) — Docker, миграции, запуск сервисов, порты, MinIO/RabbitMQ, troubleshooting.

## 5. Дополнительно


| Документ                                                     | Назначение                         |
| ------------------------------------------------------------ | ---------------------------------- |
| [REPORT.md](../REPORT.md)                                    | Отчёт о рефакторинге и архитектуре |
| [\coverage-report\index.html](..\coverage-report\index.html) | Покрытие кода тестами              |



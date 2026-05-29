# Покрытие кода тестами

Инструменты: `coverlet.collector` + `dotnet test --collect:"XPlat Code Coverage"`.

## Быстрый старт

Из корня репозитория:

**Вариант 1 — CMD (рекомендуется, обходит ExecutionPolicy):**

```cmd
scripts\Generate-CoverageReport.cmd
```

**Вариант 2 — PowerShell с Bypass (если `.ps1` блокируется):**

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Generate-CoverageReport.ps1
```

**Вариант 3 — из папки `scripts`:**

```cmd
cd scripts
Generate-CoverageReport.cmd
```

> Ошибка «выполнение сценариев отключено» — это политика Windows. Не меняйте политику глобально без необходимости; используйте `.cmd` или `-ExecutionPolicy Bypass` только для этого запуска.

Откройте отчёт в браузере:

```
coverage-report\index.html
```

## Только пересобрать HTML (без повторного запуска тестов)

Если `coverage-results` уже есть после предыдущего прогона:

```cmd
scripts\Generate-CoverageReport.cmd -SkipTest
```

## Только тесты (без отчёта)

```powershell
dotnet test PracticalWork.Library.sln
```

## С покрытием вручную (без скрипта)

```powershell
dotnet test PracticalWork.Library.sln `
  --settings CodeCoverage.runsettings `
  --collect:"XPlat Code Coverage" `
  --results-directory coverage-results
```

XML появится в `coverage-results\<guid>\coverage.cobertura.xml`.

## Что делает скрипт


| Шаг                                        | Результат                                             |
| ------------------------------------------ | ----------------------------------------------------- |
| `dotnet test` с `CodeCoverage.runsettings` | Запуск unit-тестов + сбор Cobertura XML               |
| Парсинг XML                                | Сводка по слоям Domain / Application / Infrastructure |
| HTML отчёт                                 | `coverage-report\index.html`                          |
| Копия XML                                  | `coverage-report\xml\`                                |


Фильтры сборок — в `CodeCoverage.runsettings` (без Web и миграций EF).

Русские подписи в HTML — в `scripts\coverage-labels.ru.json`.
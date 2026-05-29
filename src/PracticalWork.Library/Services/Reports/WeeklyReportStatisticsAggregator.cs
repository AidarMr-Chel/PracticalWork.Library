using System.Text.Json;
using Microsoft.Extensions.Logging;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Contracts.v1.Events.Borrows;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services.Reports;

/// <summary>
/// Агрегирует статистику выдач и возвратов из логов активности.
/// </summary>
public sealed class WeeklyReportStatisticsAggregator : IWeeklyReportStatisticsAggregator
{
    private readonly ILogger<WeeklyReportStatisticsAggregator> _logger;

    public WeeklyReportStatisticsAggregator(ILogger<WeeklyReportStatisticsAggregator> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public WeeklyStats Aggregate(IEnumerable<ActivityLogDto> events, DateTime periodStart, DateTime periodEnd)
    {
        int newBorrows = 0, returns = 0, overdue = 0;
        var errors = new List<ReportAggregationException>();
        var eventList = events.ToList();

        foreach (var e in eventList)
        {
            try
            {
                switch (e.EventType)
                {
                    case "book.borrowed":
                        var borrowed = JsonSerializer.Deserialize<BookBorrowedEvent>(e.Payload);
                        if (borrowed?.BorrowedAt == null)
                            throw new ReportAggregationException(e.EventType, e.Payload, "BorrowedAt is null");
                        if (borrowed.BorrowedAt >= periodStart && borrowed.BorrowedAt <= periodEnd)
                            newBorrows++;
                        break;

                    case "book.returned":
                        var returned = JsonSerializer.Deserialize<BookReturnedEvent>(e.Payload);
                        if (returned?.ReturnedAt == null)
                            throw new ReportAggregationException(e.EventType, e.Payload, "ReturnedAt is null");
                        if (returned.ReturnedAt >= periodStart && returned.ReturnedAt <= periodEnd)
                            returns++;
                        break;

                    default:
                        _logger.LogWarning("Неизвестный тип события: {EventType}", e.EventType);
                        break;
                }
            }
            catch (JsonException jsonEx)
            {
                errors.Add(new ReportAggregationException(e.EventType, e.Payload, "JSON deserialization failed", jsonEx));
            }
            catch (ReportAggregationException validationEx)
            {
                errors.Add(validationEx);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Unexpected error aggregating event {EventType}", e.EventType);
                errors.Add(new ReportAggregationException(e.EventType, e.Payload, "Unexpected aggregation error", ex));
            }
        }

        if (errors.Count > 0)
            throw new AggregateException(
                $"Failed to aggregate {errors.Count} out of {eventList.Count} events",
                errors);

        return new WeeklyStats(newBorrows, returns, overdue);
    }
}

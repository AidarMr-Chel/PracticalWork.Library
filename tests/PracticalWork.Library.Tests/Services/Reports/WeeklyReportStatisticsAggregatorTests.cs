using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PracticalWork.Library.Contracts.v1.Events.Borrows;
using PracticalWork.Library.Models;
using PracticalWork.Library.Services.Reports;

namespace PracticalWork.Library.Tests.Services.Reports;

public class WeeklyReportStatisticsAggregatorTests
{
    private readonly WeeklyReportStatisticsAggregator _sut =
        new(NullLogger<WeeklyReportStatisticsAggregator>.Instance);

    [Fact]
    public void Aggregate_WithSingleBorrowAndReturn_CountsCorrectly()
    {
        var start = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2026, 5, 7, 23, 59, 59, DateTimeKind.Utc);
        var borrowedAt = new DateTime(2026, 5, 3, 10, 0, 0, DateTimeKind.Utc);
        var returnedAt = new DateTime(2026, 5, 5, 12, 0, 0, DateTimeKind.Utc);

        var events = new[]
        {
            CreateEvent("book.borrowed", new BookBorrowedEvent
            {
                BorrowId = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                ReaderId = Guid.NewGuid(),
                BorrowedAt = borrowedAt,
                DueDate = borrowedAt.AddDays(30)
            }),
            CreateEvent("book.returned", new BookReturnedEvent
            {
                BorrowId = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                ReaderId = Guid.NewGuid(),
                ReturnedAt = returnedAt
            })
        };

        var stats = _sut.Aggregate(events, start, end);

        stats.NewBorrowsCount.Should().Be(1);
        stats.ReturnsCount.Should().Be(1);
    }

    [Fact]
    public void Aggregate_WithManyEvents_CountsAllInPeriod()
    {
        var start = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2026, 5, 31, 23, 59, 59, DateTimeKind.Utc);
        var events = Enumerable.Range(0, 5).Select(_ => CreateEvent("book.borrowed", new BookBorrowedEvent
        {
            BorrowId = Guid.NewGuid(),
            BookId = Guid.NewGuid(),
            ReaderId = Guid.NewGuid(),
            BorrowedAt = new DateTime(2026, 5, 10, 0, 0, 0, DateTimeKind.Utc),
            DueDate = new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc)
        })).ToList();

        var stats = _sut.Aggregate(events, start, end);

        stats.NewBorrowsCount.Should().Be(5);
    }

    [Fact]
    public void Aggregate_WhenEventOutsidePeriod_IsIgnored()
    {
        var start = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2026, 5, 7, 23, 59, 59, DateTimeKind.Utc);
        var events = new[]
        {
            CreateEvent("book.borrowed", new BookBorrowedEvent
            {
                BorrowId = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                ReaderId = Guid.NewGuid(),
                BorrowedAt = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc)
            })
        };

        var stats = _sut.Aggregate(events, start, end);

        stats.NewBorrowsCount.Should().Be(0);
    }

    [Fact]
    public void Aggregate_WhenInvalidPayload_ThrowsAggregateException()
    {
        var events = new[]
        {
            new ActivityLogDto("book.borrowed", "{ invalid json", DateTime.UtcNow)
        };

        var act = () => _sut.Aggregate(events, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);

        act.Should().Throw<AggregateException>();
    }

    private static ActivityLogDto CreateEvent<T>(string eventType, T payload) where T : class
    {
        return new ActivityLogDto(eventType, JsonSerializer.Serialize(payload), DateTime.UtcNow);
    }
}

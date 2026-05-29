using FluentAssertions;
using PracticalWork.Library.Services.Reports;
using PracticalWork.Library.Tests.Helpers;

namespace PracticalWork.Library.Tests.Services.Reports;

public class WeeklyReportPeriodProviderTests
{
    [Fact]
    public void GetLastWeekRangeUtc_OnWednesday_ReturnsPreviousCalendarWeek()
    {
        var wednesday = new DateTimeOffset(2026, 5, 13, 15, 30, 0, TimeSpan.Zero);
        var sut = new WeeklyReportPeriodProvider(new FakeTimeProvider(wednesday));

        var (start, end) = sut.GetLastWeekRangeUtc();

        start.Should().Be(new DateTime(2026, 5, 4, 0, 0, 0, DateTimeKind.Utc));
        end.Should().Be(new DateTime(2026, 5, 11, 0, 0, 0, DateTimeKind.Utc).AddTicks(-1));
    }

    [Fact]
    public void GetLastWeekRangeUtc_OnSunday_UsesMondayBasedWeek()
    {
        var sunday = new DateTimeOffset(2026, 5, 17, 10, 0, 0, TimeSpan.Zero);
        var sut = new WeeklyReportPeriodProvider(new FakeTimeProvider(sunday));

        var (start, end) = sut.GetLastWeekRangeUtc();

        start.Should().Be(new DateTime(2026, 5, 4, 0, 0, 0, DateTimeKind.Utc));
        end.Should().Be(new DateTime(2026, 5, 11, 0, 0, 0, DateTimeKind.Utc).AddTicks(-1));
    }
}

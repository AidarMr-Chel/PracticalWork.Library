using FluentAssertions;
using PracticalWork.Library.Models;
using PracticalWork.Library.Services.Reports;
using System.Text;

namespace PracticalWork.Library.Tests.Services.Reports;

public class WeeklyReportCsvBuilderTests
{
    private readonly WeeklyReportCsvBuilder _sut = new();

    [Fact]
    public void BuildCsv_ContainsStatsAndUtf8Bom()
    {
        var start = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2026, 5, 7, 23, 59, 59, DateTimeKind.Utc);
        var stats = new WeeklyStats(3, 2, 0);

        var bytes = _sut.BuildCsv(stats, start, end);

        bytes[0].Should().Be(0xEF);
        bytes[1].Should().Be(0xBB);
        bytes[2].Should().Be(0xBF);

        var text = Encoding.UTF8.GetString(bytes);
        text.Should().Contain("Новых выдач;3");
        text.Should().Contain("Возвратов;2");
    }

    [Fact]
    public void BuildEmailParameters_MapsAllKeys()
    {
        var stats = new WeeklyStats(1, 2, 3);
        var start = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2026, 5, 7, 23, 59, 59, DateTimeKind.Utc);

        var parameters = _sut.BuildEmailParameters(stats, "http://file", start, end, 7);

        parameters["NewBorrows"].Should().Be("1");
        parameters["Returns"].Should().Be("2");
        parameters["Overdue"].Should().Be("3");
        parameters["DownloadUrl"].Should().Be("http://file");
        parameters["ExpiryDays"].Should().Be("7");
    }
}

using FluentAssertions;
using Moq;
using PracticalWork.Reports.Entities;
using PracticalWork.Reports.Entities.Abstractions;
using PracticalWork.Reports.Services;

namespace PracticalWork.Reports.Tests.Services;

public class ActivityLogIngestionServiceTests
{
    [Fact]
    public async Task IngestAsync_PersistsLogViaRepository()
    {
        var repository = new Mock<IActivityLogRepository>();
        ActivityLog? saved = null;
        repository.Setup(r => r.AddAsync(It.IsAny<ActivityLog>(), default))
            .Callback<ActivityLog, CancellationToken>((log, _) => saved = log)
            .Returns(Task.CompletedTask);

        var sut = new ActivityLogIngestionService(repository.Object);

        await sut.IngestAsync("book.borrowed", "{\"id\":1}");

        saved.Should().NotBeNull();
        saved!.EventType.Should().Be("book.borrowed");
        saved.Payload.Should().Be("{\"id\":1}");
    }
}

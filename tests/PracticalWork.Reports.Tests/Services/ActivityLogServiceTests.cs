using FluentAssertions;
using Moq;
using PracticalWork.Reports.Entities.Abstractions;
using PracticalWork.Reports.Entities.DTO;
using PracticalWork.Reports.Services;

namespace PracticalWork.Reports.Tests.Services;

public class ActivityLogServiceTests
{
    [Fact]
    public async Task GetPagedAsync_DelegatesToRepository()
    {
        var filter = new ActivityLogFilterDto { Page = 2, PageSize = 50 };
        var expected = new PagedResult<ActivityLogDto>
        {
            Items =
            [
                new ActivityLogDto
                {
                    Id = Guid.NewGuid(),
                    EventType = "book.created",
                    Payload = "{}",
                    CreatedAt = DateTime.UtcNow
                }
            ],
            Total = 1,
            Page = 2,
            PageSize = 50
        };

        var repository = new Mock<IActivityLogRepository>();
        repository.Setup(r => r.GetPagedAsync(filter, default))
            .ReturnsAsync(expected);

        var sut = new ActivityLogService(repository.Object);

        var result = await sut.GetPagedAsync(filter);

        result.Should().BeSameAs(expected);
        repository.Verify(r => r.GetPagedAsync(filter, default), Times.Once);
    }
}

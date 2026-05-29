using FluentAssertions;
using Moq;
using PracticalWork.Library.Abstractions.Messaging;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Contracts.v1.Events.Readers;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;
using PracticalWork.Library.Services;
using PracticalWork.Library.Tests.Helpers;

namespace PracticalWork.Library.Tests.Services;

public class ReaderServiceTests
{
    private readonly Mock<IReaderRepository> _repository = new();
    private readonly Mock<IMessagePublisher> _publisher = new();

    private ReaderService CreateSut() => new(_repository.Object, _publisher.Object);

    [Fact]
    public async Task CreateReader_WhenPhoneIsUnique_PublishesEvent()
    {
        var reader = TestData.CreateReader();
        _repository.Setup(r => r.GetByPhoneAsync(reader.PhoneNumber)).ReturnsAsync((Reader)null!);
        _repository.Setup(r => r.AddAsync(It.IsAny<Reader>())).Returns(Task.CompletedTask);

        var id = await CreateSut().CreateReader(reader);

        id.Should().NotBeEmpty();
        reader.IsActive.Should().BeTrue();
        _publisher.Verify(p => p.PublishAsync(It.IsAny<ReaderCreatedEvent>()), Times.Once);
        _repository.Verify(r => r.AddAsync(reader), Times.Once);
    }

    [Fact]
    public async Task CreateReader_WhenPhoneExists_Throws()
    {
        var reader = TestData.CreateReader();
        _repository.Setup(r => r.GetByPhoneAsync(reader.PhoneNumber))
            .ReturnsAsync(TestData.CreateReader());

        var act = () => CreateSut().CreateReader(reader);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*уже существует*");
    }

    [Fact]
    public async Task ExtendReader_WhenNotFound_Throws()
    {
        var id = Guid.NewGuid();
        _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Reader)null!);

        var act = () => CreateSut().ExtendReader(id, DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2)));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"*id={id}*");
    }

    [Fact]
    public async Task CloseReader_WhenHasUnreturnedBooks_Throws()
    {
        var id = Guid.NewGuid();
        var reader = TestData.CreateReader(id: id);
        var books = new List<Book> { TestData.CreateBook(status: BookStatus.Borrow) };

        _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(reader);
        _repository.Setup(r => r.GetBooksByReaderIdAsync(id)).ReturnsAsync(books);

        var act = () => CreateSut().CloseReader(id);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*несданные книги*");
    }

    [Fact]
    public async Task CloseReader_WhenNoBooks_DeactivatesAndPublishes()
    {
        var id = Guid.NewGuid();
        var reader = TestData.CreateReader(id: id);
        _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(reader);
        _repository.Setup(r => r.GetBooksByReaderIdAsync(id))
            .ReturnsAsync(new List<Book>());

        await CreateSut().CloseReader(id);

        reader.IsActive.Should().BeFalse();
        _repository.Verify(r => r.UpdateAsync(reader), Times.Once);
        _publisher.Verify(p => p.PublishAsync(It.IsAny<ReaderClosedEvent>()), Times.Once);
    }
}

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Models;
using PracticalWork.Library.Services;
using PracticalWork.Library.Tests.Helpers;

namespace PracticalWork.Library.Tests.Services;

public class ArchiveBooksWorkflowTests
{
    private readonly Mock<IBookService> _bookService = new();
    private readonly Mock<IBookRepository> _bookRepository = new();
    private readonly Mock<IMinioService> _minio = new();

    [Fact]
    public async Task ExecuteAsync_WhenNoBooks_ReturnsSuccessWithZeroProcessed()
    {
        _bookRepository.Setup(r => r.GetArchivableBooksAsync(3, 100, default))
            .ReturnsAsync(Array.Empty<Book>());

        var result = await CreateSut().ExecuteAsync();

        result.IsSuccess.Should().BeTrue();
        result.ProcessedCount.Should().Be(0);
        result.ErrorCount.Should().Be(0);
        _bookService.Verify(s => s.ArchivingBook(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBooksFound_ArchivesAndDeletesCover()
    {
        var book = TestData.CreateBook();
        book.CoverImagePath = "covers/test.png";

        _bookRepository.Setup(r => r.GetArchivableBooksAsync(3, 100, default))
            .ReturnsAsync(new[] { book });
        _bookService.Setup(s => s.ArchivingBook(book.Id))
            .ReturnsAsync(TestData.CreateBook(id: book.Id, status: Enums.BookStatus.Archived));

        var result = await CreateSut().ExecuteAsync();

        result.ProcessedCount.Should().Be(1);
        result.ErrorCount.Should().Be(0);
        result.IsSuccess.Should().BeTrue();
        _bookService.Verify(s => s.ArchivingBook(book.Id), Times.Once);
        _minio.Verify(m => m.DeleteAsync(book.CoverImagePath), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCoverDeleteFails_StillCountsAsSuccess()
    {
        var book = TestData.CreateBook();
        book.CoverImagePath = "covers/test.png";

        _bookRepository.Setup(r => r.GetArchivableBooksAsync(3, 100, default))
            .ReturnsAsync(new[] { book });
        _bookService.Setup(s => s.ArchivingBook(book.Id))
            .ReturnsAsync(TestData.CreateBook(id: book.Id));
        _minio.Setup(m => m.DeleteAsync(book.CoverImagePath))
            .ThrowsAsync(new InvalidOperationException("minio down"));

        var result = await CreateSut().ExecuteAsync();

        result.ProcessedCount.Should().Be(1);
        result.ErrorCount.Should().Be(0);
    }

    [Fact]
    public async Task ExecuteAsync_WhenArchiveFails_IncrementsErrorCount()
    {
        var book = TestData.CreateBook();
        _bookRepository.Setup(r => r.GetArchivableBooksAsync(3, 100, default))
            .ReturnsAsync(new[] { book });
        _bookService.Setup(s => s.ArchivingBook(book.Id))
            .ThrowsAsync(new InvalidOperationException("db error"));

        var result = await CreateSut().ExecuteAsync();

        result.ProcessedCount.Should().Be(0);
        result.ErrorCount.Should().Be(1);
        result.IsSuccess.Should().BeFalse();
    }

    private ArchiveBooksWorkflow CreateSut()
    {
        var settings = Options.Create(new ArchiveSettings
        {
            YearsWithoutBorrow = 3,
            MaxBooksPerRun = 100
        });

        return new ArchiveBooksWorkflow(
            _bookService.Object,
            _bookRepository.Object,
            _minio.Object,
            settings,
            new FakeTimeProvider(new DateTimeOffset(2026, 5, 15, 12, 0, 0, TimeSpan.Zero)),
            NullLogger<ArchiveBooksWorkflow>.Instance);
    }
}

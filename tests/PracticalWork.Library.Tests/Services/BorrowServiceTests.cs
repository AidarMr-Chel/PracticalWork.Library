using FluentAssertions;
using Moq;
using PracticalWork.Library.Abstractions.Messaging;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Contracts.v1.Events.Borrows;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;
using PracticalWork.Library.Services;
using PracticalWork.Library.Tests.Helpers;

namespace PracticalWork.Library.Tests.Services;

public class BorrowServiceTests
{
    private readonly Mock<IBorrowRepository> _borrowRepository = new();
    private readonly Mock<IBookRepository> _bookRepository = new();
    private readonly Mock<IReaderRepository> _readerRepository = new();
    private readonly Mock<IMinioService> _minio = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly Mock<IMessagePublisher> _publisher = new();

    private BorrowService CreateSut() =>
        new(
            _borrowRepository.Object,
            _bookRepository.Object,
            _readerRepository.Object,
            _minio.Object,
            _cache.Object,
            _publisher.Object);

    [Fact]
    public async Task CreateBorrow_WhenSuccess_UpdatesBookAndPublishesEvent()
    {
        var bookId = Guid.NewGuid();
        var readerId = Guid.NewGuid();
        var borrowId = Guid.NewGuid();
        var book = TestData.CreateBook(id: bookId, status: BookStatus.Available);
        var reader = TestData.CreateReader(id: readerId);

        _bookRepository.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
        _readerRepository.Setup(r => r.GetByIdAsync(readerId)).ReturnsAsync(reader);
        _borrowRepository.Setup(r => r.AddBorrowAsync(It.IsAny<Borrow>())).ReturnsAsync(borrowId);

        var result = await CreateSut().CreateBorrow(bookId, readerId);

        result.Should().Be(borrowId);
        book.Status.Should().Be(BookStatus.Borrow);
        _bookRepository.Verify(r => r.UpdateAsync(book), Times.Once);
        _publisher.Verify(p => p.PublishAsync(It.IsAny<BookBorrowedEvent>()), Times.Once);
        _cache.Verify(c => c.ClearByRegistryAsync("Borrow:Keys"), Times.Once);
    }

    [Fact]
    public async Task CreateBorrow_WhenBookAlreadyBorrowed_Throws()
    {
        var bookId = Guid.NewGuid();
        var readerId = Guid.NewGuid();
        _bookRepository.Setup(r => r.GetByIdAsync(bookId))
            .ReturnsAsync(TestData.CreateBook(id: bookId, status: BookStatus.Borrow));
        _readerRepository.Setup(r => r.GetByIdAsync(readerId))
            .ReturnsAsync(TestData.CreateReader(id: readerId));

        var act = () => CreateSut().CreateBorrow(bookId, readerId);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*уже выдана*");
    }

    [Fact]
    public async Task CreateBorrow_WhenReaderInactive_Throws()
    {
        var bookId = Guid.NewGuid();
        var readerId = Guid.NewGuid();
        _bookRepository.Setup(r => r.GetByIdAsync(bookId))
            .ReturnsAsync(TestData.CreateBook(id: bookId));
        _readerRepository.Setup(r => r.GetByIdAsync(readerId))
            .ReturnsAsync(TestData.CreateReader(id: readerId, isActive: false));

        var act = () => CreateSut().CreateBorrow(bookId, readerId);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*неактивна*");
    }

    [Fact]
    public async Task ReturnBook_WhenNoActiveBorrow_Throws()
    {
        var bookId = Guid.NewGuid();
        _borrowRepository.Setup(r => r.GetActiveBorrowAsync(bookId))
            .ReturnsAsync((Borrow)null!);

        var act = () => CreateSut().ReturnBook(bookId);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*активной выдачи*");
    }

    [Fact]
    public async Task GetAvailableBooks_WhenCached_DoesNotCallRepository()
    {
        var filter = TestData.CreateBook();
        var cached = new List<Book> { filter };
        _cache.Setup(c => c.GetAsync<IEnumerable<Book>>(It.IsAny<string>()))
            .ReturnsAsync(cached);

        var result = await CreateSut().GetAvailableBooksAsync(filter);

        result.Should().BeEquivalentTo(cached);
        _bookRepository.Verify(r => r.FindAsync(
            It.IsAny<Book>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateBorrow_WhenBookArchived_Throws()
    {
        var bookId = Guid.NewGuid();
        var readerId = Guid.NewGuid();
        _bookRepository.Setup(r => r.GetByIdAsync(bookId))
            .ReturnsAsync(TestData.CreateBook(id: bookId, status: BookStatus.Archived));

        var act = () => CreateSut().CreateBorrow(bookId, readerId);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*архиве*");
    }

    [Fact]
    public async Task ReturnBook_WhenActiveBorrow_UpdatesStatusesAndPublishesEvent()
    {
        var bookId = Guid.NewGuid();
        var borrowId = Guid.NewGuid();
        var borrow = new Borrow
        {
            Id = borrowId,
            BookId = bookId,
            ReaderId = Guid.NewGuid(),
            BorrowDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20)),
            Status = BookIssueStatus.Issued
        };
        var book = TestData.CreateBook(id: bookId, status: BookStatus.Borrow);

        _borrowRepository.Setup(r => r.GetActiveBorrowAsync(bookId)).ReturnsAsync(borrow);
        _bookRepository.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);

        await CreateSut().ReturnBook(bookId);

        borrow.Status.Should().Be(BookIssueStatus.Returned);
        borrow.ReturnDate.Should().NotBe(default);
        book.Status.Should().Be(BookStatus.Available);
        _borrowRepository.Verify(r => r.UpdateBorrowAsync(borrow), Times.Once);
        _publisher.Verify(p => p.PublishAsync(It.IsAny<BookReturnedEvent>()), Times.Once);
        _cache.Verify(c => c.ClearByRegistryAsync("Borrow:Keys"), Times.Once);
        _cache.Verify(c => c.ClearByRegistryAsync("AvailableBooks:Keys"), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCached_DoesNotCallRepository()
    {
        var id = Guid.NewGuid();
        var cached = new Borrow { Id = id, BookId = Guid.NewGuid(), ReaderId = Guid.NewGuid() };
        _cache.Setup(c => c.GetAsync<Borrow>($"Borrow:{id}")).ReturnsAsync(cached);

        var result = await CreateSut().GetByIdAsync(id);

        result.Should().BeSameAs(cached);
        _borrowRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetDetailsAsync_ByReaderName_DelegatesToRepository()
    {
        var readerId = Guid.NewGuid();
        var reader = TestData.CreateReader(id: readerId);
        var borrow = new Borrow
        {
            Id = Guid.NewGuid(),
            BookId = Guid.NewGuid(),
            ReaderId = readerId,
            BorrowDate = DateOnly.FromDateTime(DateTime.UtcNow),
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
            Status = BookIssueStatus.Issued
        };

        _readerRepository.Setup(r => r.GetByNameAsync(reader.FullName)).ReturnsAsync(reader);
        _borrowRepository.Setup(r => r.GetByReaderIdAsync(readerId)).ReturnsAsync(borrow);
        _cache.Setup(c => c.GetAsync<Borrow>(It.IsAny<string>())).ReturnsAsync((Borrow)null!);

        var result = await CreateSut().GetDetailsAsync(reader.FullName);

        result.Should().BeSameAs(borrow);
    }

    [Fact]
    public async Task GetBorrowDetailsAsync_WhenFound_RequestsCoverUrl()
    {
        var borrowId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var borrow = new Borrow
        {
            Id = borrowId,
            BookId = bookId,
            ReaderId = Guid.NewGuid(),
            BorrowDate = DateOnly.FromDateTime(DateTime.UtcNow),
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
            Status = BookIssueStatus.Issued
        };

        _cache.Setup(c => c.GetAsync<Borrow>(It.IsAny<string>())).ReturnsAsync((Borrow)null!);
        _borrowRepository.Setup(r => r.GetByIdAsync(borrowId)).ReturnsAsync(borrow);
        _minio.Setup(m => m.GetFileUrlAsync(It.IsAny<string>())).ReturnsAsync("http://cover");

        var result = await CreateSut().GetBorrowDetailsAsync(borrowId.ToString());

        result.Should().NotBeNull();
        result!.CoverUrl.Should().Be("http://cover");
        _minio.Verify(m => m.GetFileUrlAsync($"covers/{bookId}/cover.png"), Times.Once);
    }
}

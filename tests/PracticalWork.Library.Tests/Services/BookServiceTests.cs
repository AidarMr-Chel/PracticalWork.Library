using FluentAssertions;
using Moq;
using PracticalWork.Library.Abstractions.Messaging;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Contracts.v1.Events.Books;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models;
using PracticalWork.Library.Tests.Helpers;

namespace PracticalWork.Library.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _repository = new();
    private readonly Mock<IMinioService> _minio = new();
    private readonly Mock<ICacheService> _cache = new();
    private readonly Mock<IMessagePublisher> _publisher = new();

    private BookService CreateSut() =>
        new(_repository.Object, _minio.Object, _cache.Object, _publisher.Object);

    [Fact]
    public async Task CreateBook_SetsAvailableStatus_PublishesEvent_AndClearsCache()
    {
        var book = TestData.CreateBook();
        var newId = Guid.NewGuid();
        _repository.Setup(r => r.AddAsync(It.IsAny<Book>())).ReturnsAsync(newId);

        var result = await CreateSut().CreateBook(book);

        result.Should().Be(newId);
        book.Status.Should().Be(BookStatus.Available);
        _publisher.Verify(p => p.PublishAsync(It.IsAny<BookCreatedEvent>()), Times.Once);
        _cache.Verify(c => c.ClearByRegistryAsync("Books:Keys"), Times.Once);
    }

    [Fact]
    public async Task UpdateBook_WhenNotFound_Throws()
    {
        var id = Guid.NewGuid();
        _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Book)null!);

        var act = () => CreateSut().UpdateBook(id, TestData.CreateBook(id: id));

        await act.Should().ThrowAsync<BookServiceException>()
            .WithMessage($"*id={id}*");
    }

    [Fact]
    public async Task UpdateBook_WhenCategoryChanged_Throws()
    {
        var id = Guid.NewGuid();
        var existing = TestData.CreateBook(id: id, category: BookCategory.FictionBook);
        var update = TestData.CreateBook(id: id, category: BookCategory.ScientificBook);
        _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

        var act = () => CreateSut().UpdateBook(id, update);

        await act.Should().ThrowAsync<BookServiceException>()
            .WithMessage("*категорию*");
    }

    [Fact]
    public async Task UpdateBook_WhenValid_UpdatesRepository_AndInvalidatesCache()
    {
        var id = Guid.NewGuid();
        var existing = TestData.CreateBook(id: id);
        var update = TestData.CreateBook(id: id);
        _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

        await CreateSut().UpdateBook(id, update);

        _repository.Verify(r => r.UpdateAsync(It.Is<Book>(b => b.Id == id)), Times.Once);
        _cache.Verify(c => c.ClearByRegistryAsync("Books:Keys"), Times.Once);
        _cache.Verify(c => c.RemoveAsync($"Book:{id}:Details"), Times.Once);
    }

    [Fact]
    public async Task GetBooks_WhenCached_ReturnsFromCacheWithoutRepository()
    {
        var filter = TestData.CreateBook();
        var cached = new List<Book> { filter };
        _cache.Setup(c => c.GetAsync<IEnumerable<Book>>(It.IsAny<string>()))
            .ReturnsAsync(cached);

        var result = await CreateSut().GetBooks(filter);

        result.Should().BeEquivalentTo(cached);
        _repository.Verify(r => r.FindPagedAsync(
            It.IsAny<Book>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetBooks_WhenNotCached_LoadsFromRepository_AndStoresInCache()
    {
        var filter = TestData.CreateBook();
        var books = new List<Book> { filter };
        _cache.Setup(c => c.GetAsync<IEnumerable<Book>>(It.IsAny<string>()))
            .ReturnsAsync((IEnumerable<Book>)null!);
        _repository.Setup(r => r.FindPagedAsync(filter, 1, 10, false, default))
            .ReturnsAsync(books);

        var result = await CreateSut().GetBooks(filter);

        result.Should().BeEquivalentTo(books);
        _cache.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<Book>>(), It.IsAny<TimeSpan>()), Times.Once);
        _cache.Verify(c => c.TrackKeyAsync("Books:Keys", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ArchivingBook_WhenFound_ArchivesPublishesAndClearsCache()
    {
        var id = Guid.NewGuid();
        var book = TestData.CreateBook(id: id);
        _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(book);

        var result = await CreateSut().ArchivingBook(id);

        result.Status.Should().Be(BookStatus.Archived);
        _repository.Verify(r => r.UpdateAsync(It.Is<Book>(b => b.IsArchived)), Times.Once);
        _publisher.Verify(p => p.PublishAsync(It.IsAny<BookArchivedEvent>()), Times.Once);
        _cache.Verify(c => c.ClearByRegistryAsync("Books:Keys"), Times.Once);
        _cache.Verify(c => c.RemoveAsync($"Book:{id}:Details"), Times.Once);
    }

    [Fact]
    public async Task UpdateBookDetails_WhenInvalidContentType_Throws()
    {
        var id = Guid.NewGuid();
        _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(TestData.CreateBook(id: id));
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        var act = () => CreateSut().UpdateBookDetailsAsync(id, "desc", stream, "f.txt", "text/plain");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*изображением*");
    }
}

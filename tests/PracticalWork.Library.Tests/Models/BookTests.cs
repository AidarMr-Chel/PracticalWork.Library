using FluentAssertions;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;
using PracticalWork.Library.Tests.Helpers;

namespace PracticalWork.Library.Tests.Models;

public class BookTests
{
    [Fact]
    public void CanBeBorrowed_WhenAvailableAndNotArchived_ReturnsTrue()
    {
        var book = TestData.CreateBook(status: BookStatus.Available);

        book.CanBeBorrowed().Should().BeTrue();
    }

    [Fact]
    public void CanBeBorrowed_WhenArchived_ReturnsFalse()
    {
        var book = TestData.CreateBook();
        book.Archive();

        book.CanBeBorrowed().Should().BeFalse();
    }

    [Fact]
    public void Archive_WhenBookIsBorrowed_Throws()
    {
        var book = TestData.CreateBook(status: BookStatus.Borrow);

        var act = () => book.Archive();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*не может быть заархивирована*");
    }

    [Fact]
    public void Archive_WhenAvailable_SetsArchivedStatus()
    {
        var book = TestData.CreateBook(status: BookStatus.Available);

        book.Archive();

        book.IsArchived.Should().BeTrue();
        book.Status.Should().Be(BookStatus.Archived);
    }
}

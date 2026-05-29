using FluentAssertions;
using PracticalWork.Library.Controllers.Validations.v1;

namespace PracticalWork.Library.Tests.Validation;

public class CreateBorrowQueryValidatorTests
{
    private readonly CreateBorrowQueryValidator _validator = new();

    [Fact]
    public void ValidQuery_PassesValidation()
    {
        var query = new CreateBorrowQuery
        {
            BookId = Guid.NewGuid(),
            ReaderId = Guid.NewGuid()
        };

        _validator.Validate(query).IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyBookId_FailsValidation()
    {
        var query = new CreateBorrowQuery
        {
            BookId = Guid.Empty,
            ReaderId = Guid.NewGuid()
        };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateBorrowQuery.BookId));
    }

    [Fact]
    public void EmptyReaderId_FailsValidation()
    {
        var query = new CreateBorrowQuery
        {
            BookId = Guid.NewGuid(),
            ReaderId = Guid.Empty
        };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateBorrowQuery.ReaderId));
    }
}

using FluentAssertions;
using PracticalWork.Library.Controllers.Validations.v1;

namespace PracticalWork.Library.Tests.Validation;

public class ReturnBookQueryValidatorTests
{
    private readonly ReturnBookQueryValidator _validator = new();

    [Fact]
    public void ValidQuery_PassesValidation()
    {
        var query = new ReturnBookQuery { BookId = Guid.NewGuid() };

        _validator.Validate(query).IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyBookId_FailsValidation()
    {
        var query = new ReturnBookQuery { BookId = Guid.Empty };

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ReturnBookQuery.BookId));
    }
}

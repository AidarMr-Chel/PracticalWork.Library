using FluentAssertions;
using PracticalWork.Library.Contracts.v1.Books.Request;
using PracticalWork.Library.Contracts.v1.Enums;
using PracticalWork.Library.Controllers.Validations.v1;

namespace PracticalWork.Library.Tests.Validation;

public class CreateBookRequestValidatorTests
{
    private readonly CreateBookRequestValidator _validator = new();

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        var request = new CreateBookRequest(
            "Title",
            BookCategory.FictionBook,
            new[] { "Author" },
            "Description",
            2020);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyTitle_FailsValidation()
    {
        var request = new CreateBookRequest(
            "",
            BookCategory.FictionBook,
            new[] { "Author" },
            null!,
            2020);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void YearOutOfRange_FailsValidation()
    {
        var request = new CreateBookRequest(
            "Title",
            BookCategory.FictionBook,
            new[] { "Author" },
            null!,
            1700);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Year");
    }
}

using FluentAssertions;
using PracticalWork.Library.Contracts.v1.Books.Request;
using PracticalWork.Library.Controllers.Validations.v1;

namespace PracticalWork.Library.Tests.Validation;

public class BookFilterRequestValidatorTests
{
    private readonly BookFilterRequestValidator _validator = new();

    private static BookFilterRequest ValidRequest(int pageNumber = 1, int pageSize = 10, int year = 0) =>
        new("Title", null, Array.Empty<string>(), year, null, null, "", pageNumber, pageSize);

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        var result = _validator.Validate(ValidRequest());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void PageNumberZero_FailsValidation()
    {
        var result = _validator.Validate(ValidRequest(pageNumber: 0));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public void PageSizeOutOfRange_FailsValidation()
    {
        var result = _validator.Validate(ValidRequest(pageSize: 200));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
    }
}

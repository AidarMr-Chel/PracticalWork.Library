using FluentAssertions;
using PracticalWork.Library.Contracts.v1.Readers.Request;
using PracticalWork.Library.Controllers.Validations.v1;

namespace PracticalWork.Library.Tests.Validation;

public class CreateReaderRequestValidatorTests
{
    private readonly CreateReaderRequestValidator _validator = new();

    private static CreateReaderRequest ValidRequest() =>
        new(
            "Ivan Ivanov",
            "+79001234567",
            "ivan@example.com",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)));

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        _validator.Validate(ValidRequest()).IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyFullName_FailsValidation()
    {
        var request = ValidRequest() with { FullName = "" };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateReaderRequest.FullName));
    }

    [Fact]
    public void InvalidEmail_FailsValidation()
    {
        var request = ValidRequest() with { Email = "not-an-email" };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateReaderRequest.Email));
    }

    [Fact]
    public void ExpiryDateInPast_FailsValidation()
    {
        var request = ValidRequest() with
        {
            ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateReaderRequest.ExpiryDate));
    }
}

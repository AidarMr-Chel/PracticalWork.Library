using FluentAssertions;
using PracticalWork.Reports.Entities.DTO;
using PracticalWork.Reports.Web.Validations;

namespace PracticalWork.Reports.Tests.Validation;

public class GenerateReportRequestValidatorTests
{
    private readonly GenerateReportRequestValidator _validator = new();

    [Fact]
    public void ValidRequest_Passes()
    {
        var request = new GenerateReportRequest
        {
            From = new DateOnly(2026, 5, 1),
            To = new DateOnly(2026, 5, 10)
        };

        _validator.Validate(request).IsValid.Should().BeTrue();
    }

    [Fact]
    public void ToBeforeFrom_Fails()
    {
        var request = new GenerateReportRequest
        {
            From = new DateOnly(2026, 5, 10),
            To = new DateOnly(2026, 5, 1)
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
    }
}

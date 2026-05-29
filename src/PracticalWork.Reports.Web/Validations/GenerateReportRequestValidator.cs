using FluentValidation;
using PracticalWork.Reports.Entities.DTO;

namespace PracticalWork.Reports.Web.Validations;

/// <summary>
/// Валидатор запроса на генерацию отчёта.
/// </summary>
public sealed class GenerateReportRequestValidator : AbstractValidator<GenerateReportRequest>
{
    public GenerateReportRequestValidator()
    {
        RuleFor(x => x.From)
            .Must(x => x != default)
            .WithMessage("Дата начала периода обязательна.");

        RuleFor(x => x.To)
            .Must(x => x != default)
            .WithMessage("Дата окончания периода обязательна.")
            .GreaterThanOrEqualTo(x => x.From)
            .WithMessage("Дата окончания не может быть раньше даты начала.");
    }
}

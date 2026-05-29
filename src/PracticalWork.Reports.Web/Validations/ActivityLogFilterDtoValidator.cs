using FluentValidation;
using PracticalWork.Reports.Entities.DTO;

namespace PracticalWork.Reports.Web.Validations;

/// <summary>
/// Валидатор фильтра логов активности.
/// </summary>
public sealed class ActivityLogFilterDtoValidator : AbstractValidator<ActivityLogFilterDto>
{
    public ActivityLogFilterDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Номер страницы должен быть больше 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Размер страницы должен быть от 1 до 100.");

        RuleFor(x => x.To)
            .GreaterThanOrEqualTo(x => x.From!.Value)
            .When(x => x.From.HasValue && x.To.HasValue)
            .WithMessage("Верхняя граница даты не может быть раньше нижней.");
    }
}

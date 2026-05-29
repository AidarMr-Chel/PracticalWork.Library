using FluentValidation;
using PracticalWork.Library.Contracts.v1.Readers.Request;

namespace PracticalWork.Library.Controllers.Validations.v1;

/// <summary>
/// Валидатор запроса на продление читательского билета.
/// </summary>
public sealed class ExtendReaderRequestValidator : AbstractValidator<ExtendReaderRequest>
{
    public ExtendReaderRequestValidator()
    {
        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Новая дата окончания должна быть в будущем.");
    }
}

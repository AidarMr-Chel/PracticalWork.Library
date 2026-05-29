using FluentValidation;
using PracticalWork.Library.Contracts.v1.Readers.Request;

namespace PracticalWork.Library.Controllers.Validations.v1;

/// <summary>
/// Валидатор запроса на создание читателя.
/// </summary>
public sealed class CreateReaderRequestValidator : AbstractValidator<CreateReaderRequest>
{
    public CreateReaderRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("ФИО обязательно.")
            .MaximumLength(200);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Номер телефона обязателен.")
            .MaximumLength(20);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен.")
            .EmailAddress().WithMessage("Некорректный email.");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Дата окончания должна быть в будущем.");
    }
}

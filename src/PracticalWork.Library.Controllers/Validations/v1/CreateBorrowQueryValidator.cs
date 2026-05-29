using FluentValidation;

namespace PracticalWork.Library.Controllers.Validations.v1;

/// <summary>
/// Модель query-параметров для создания выдачи.
/// </summary>
public sealed class CreateBorrowQuery
{
    public Guid BookId { get; init; }
    public Guid ReaderId { get; init; }
}

/// <summary>
/// Валидатор параметров создания выдачи.
/// </summary>
public sealed class CreateBorrowQueryValidator : AbstractValidator<CreateBorrowQuery>
{
    public CreateBorrowQueryValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Идентификатор книги обязателен.");

        RuleFor(x => x.ReaderId)
            .NotEmpty().WithMessage("Идентификатор читателя обязателен.");
    }
}

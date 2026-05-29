using FluentValidation;

namespace PracticalWork.Library.Controllers.Validations.v1;

/// <summary>
/// Модель query-параметров для возврата книги.
/// </summary>
public sealed class ReturnBookQuery
{
    public Guid BookId { get; init; }
}

/// <summary>
/// Валидатор параметров возврата книги.
/// </summary>
public sealed class ReturnBookQueryValidator : AbstractValidator<ReturnBookQuery>
{
    public ReturnBookQueryValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Идентификатор книги обязателен.");
    }
}

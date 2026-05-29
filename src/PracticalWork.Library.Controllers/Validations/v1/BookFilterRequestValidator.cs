using FluentValidation;
using PracticalWork.Library.Contracts.v1.Books.Request;

namespace PracticalWork.Library.Controllers.Validations.v1;

/// <summary>
/// Валидатор параметров фильтрации списка книг.
/// </summary>
public sealed class BookFilterRequestValidator : AbstractValidator<BookFilterRequest>
{
    public BookFilterRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Номер страницы должен быть больше 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Размер страницы должен быть от 1 до 100.");

        RuleFor(x => x.Year)
            .InclusiveBetween(0, DateTime.Now.Year)
            .When(x => x.Year > 0)
            .WithMessage("Год издания указан некорректно.");
    }
}

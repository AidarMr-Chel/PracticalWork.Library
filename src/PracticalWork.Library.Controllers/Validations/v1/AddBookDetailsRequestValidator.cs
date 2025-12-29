using FluentValidation;
using PracticalWork.Library.Contracts.v1.Books.Request;

namespace PracticalWork.Library.Controllers.Validations.v1
{
    /// <summary>
    /// Валидатор запроса обновления деталей книги.
    /// Проверяет корректность описания и файла обложки.
    /// </summary>
    public sealed class AddBookDetailsRequestValidator : AbstractValidator<UpdateBookDetailsRequest>
    {
        /// <summary>
        /// Создаёт новый экземпляр валидатора для <see cref="UpdateBookDetailsRequest"/>.
        /// </summary>
        public AddBookDetailsRequestValidator()
        {
            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Описание не может превышать 2000 символов.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.CoverFile)
                .Must(file => file == null || file.Length <= 5_000_000)
                .WithMessage("Размер файла обложки не может превышать 5 MB.");

            RuleFor(x => x.CoverFile)
                .Must(file => file == null || file.ContentType.StartsWith("image/"))
                .WithMessage("Файл обложки должен быть изображением (image/jpeg, image/png и т.п.).");
        }
    }
}

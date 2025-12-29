using FluentValidation;
using PracticalWork.Library.Contracts.v1.Books.Request;
using Microsoft.AspNetCore.Http;

namespace PracticalWork.Library.Controllers.Validations.v1
{
    public sealed class AddBookDetailsRequestValidator : AbstractValidator<UpdateBookDetailsRequest>
    {
        /// <summary>
        /// Конструктор валидатора запроса добавления деталей книги
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

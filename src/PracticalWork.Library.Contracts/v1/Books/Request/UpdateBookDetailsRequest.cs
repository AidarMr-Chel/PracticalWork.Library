using Microsoft.AspNetCore.Http;

namespace PracticalWork.Library.Contracts.v1.Books.Request
{
    /// <summary>
    /// Запрос на обновление деталей книги.
    /// Позволяет изменить описание и загрузить новую обложку.
    /// </summary>
    public sealed class UpdateBookDetailsRequest
    {
        /// <summary>
        /// Новое описание книги.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Файл обложки книги.
        /// Поддерживаемые форматы: image/jpeg, image/png и другие изображения.
        /// </summary>
        public IFormFile CoverFile { get; set; }
    }
}

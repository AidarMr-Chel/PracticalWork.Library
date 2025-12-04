using Microsoft.AspNetCore.Http;

namespace PracticalWork.Library.Contracts.v1.Books.Request
{
    public sealed class UpdateBookDetailsRequest
    {
        /// <summary>
        /// Новое описание книги
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Файл обложки (image/jpeg, image/png и т.п.)
        /// </summary>
        public IFormFile CoverFile { get; set; }
    }
}

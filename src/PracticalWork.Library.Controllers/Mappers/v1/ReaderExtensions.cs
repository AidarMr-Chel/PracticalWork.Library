using PracticalWork.Library.Contracts.v1.Readers.Request;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Controllers.Mappers.v1
{
    /// <summary>
    /// Методы расширения для преобразования запросов,
    /// связанных с читателями, в доменные модели.
    /// </summary>
    public static class ReaderExtensions
    {
        /// <summary>
        /// Преобразует запрос на создание читателя
        /// в доменную модель <see cref="Reader"/>.
        /// </summary>
        /// <param name="request">Запрос на создание читателя.</param>
        /// <returns>Экземпляр доменной модели читателя.</returns>
        public static Reader ToReader(this CreateReaderRequest request) =>
            new()
            {
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                ExpiryDate = request.ExpiryDate
            };
    }
}

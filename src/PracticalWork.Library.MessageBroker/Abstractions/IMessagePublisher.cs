using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.MessageBroker.Abstractions
{
    /// <summary>
    /// Публикатор событий доменной модели.
    /// </summary>
    public interface IMessagePublisher
    {
        /// <summary>
        /// Публикация события в брокер сообщений.
        /// </summary>
        /// <param name="evt">Событие, наследующее BaseLibraryEvent</param>
        Task PublishAsync(BaseLibraryEvent evt);
    }
}

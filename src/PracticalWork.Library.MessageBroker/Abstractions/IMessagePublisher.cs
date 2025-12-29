using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.MessageBroker.Abstractions
{
    /// <summary>
    /// Интерфейс для публикации событий доменной модели
    /// в брокер сообщений.
    /// </summary>
    public interface IMessagePublisher
    {
        /// <summary>
        /// Публикует событие в брокер сообщений.
        /// </summary>
        /// <param name="evt">Событие, наследующее <see cref="BaseLibraryEvent"/>.</param>
        Task PublishAsync(BaseLibraryEvent evt);
    }
}

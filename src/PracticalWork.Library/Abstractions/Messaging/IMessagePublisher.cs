using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.Abstractions.Messaging;

/// <summary>
/// Порт публикации доменных событий во внешний брокер сообщений.
/// </summary>
public interface IMessagePublisher
{
    Task PublishAsync(BaseLibraryEvent evt);
}

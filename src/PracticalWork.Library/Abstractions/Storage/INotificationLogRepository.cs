using System;
using System.Threading.Tasks;

namespace PracticalWork.Library.Abstractions.Storage;

public interface INotificationLogRepository
{
    Task<bool> WasNotifiedRecentlyAsync(Guid borrowId, string type, TimeSpan timeWindow);
    Task AddAsync(string type, Guid borrowId, string status, string errorMessage = null);
    Task SaveChangesAsync();
}
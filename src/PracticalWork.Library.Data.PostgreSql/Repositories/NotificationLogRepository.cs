using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

public sealed class NotificationLogRepository : INotificationLogRepository
{
    private readonly AppDbContext _context;

    public NotificationLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> WasNotifiedRecentlyAsync(Guid borrowId, string type, TimeSpan timeWindow)
    {
        var threshold = DateTime.UtcNow - timeWindow;

        return await _context.NotificationLogs
            .AnyAsync(n => n.BorrowId == borrowId
                        && n.Type == type
                        && n.SentAt >= threshold);
    }

    public async Task AddAsync(string type, Guid borrowId, string status, string errorMessage = null)
    {
        await _context.NotificationLogs.AddAsync(new NotificationLogEntity
        {
            Id = Guid.NewGuid(),
            BorrowId = borrowId,
            Type = type,
            SentAt = DateTime.UtcNow,
            Status = status,
            ErrorMessage = errorMessage
        });
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
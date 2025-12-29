using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PracticalWork.Library.Data.PostgreSql;

/// <summary>
/// Утилита для применения миграций базы данных при запуске приложения.
/// Позволяет централизованно выполнять обновление схемы БД.
/// </summary>
public static class MigrationsRunner
{
    /// <summary>
    /// Применяет все ожидающие миграции к базе данных.
    /// Создаёт отдельный scope, получает <see cref="AppDbContext"/> и вызывает <c>MigrateAsync()</c>.
    /// </summary>
    /// <param name="logger">Логгер для вывода диагностической информации.</param>
    /// <param name="serviceProvider">Провайдер сервисов приложения.</param>
    /// <param name="appName">Имя приложения, используемое в логах.</param>
    public static async Task ApplyMigrations(ILogger logger, IServiceProvider serviceProvider, string appName)
    {
        var operationId = Guid.NewGuid().ToString()[..4];
        logger.LogInformation($"{appName}:UpdateDatabase:{operationId}: starting...");

        try
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();
            }

            logger.LogInformation($"{appName}:UpdateDatabase:{operationId}: successfully done");
            await Task.FromResult(true);
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, $"{appName}:UpdateDatabase.{operationId}: Migration failed");
            throw;
        }
    }
}

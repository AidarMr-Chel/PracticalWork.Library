using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Data.PostgreSql.Entities;

namespace PracticalWork.Library.Data.PostgreSql;

/// <summary>
/// Контекст отчётной базы данных.
/// Содержит таблицы, используемые для аналитики и логирования.
/// </summary>
public class ReportsDbContext : DbContext
{
    /// <summary>
    /// Создаёт экземпляр контекста отчётной БД.
    /// </summary>
    /// <param name="options">Параметры конфигурации DbContext.</param>
    public ReportsDbContext(DbContextOptions<ReportsDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Логи активности, используемые для аналитики и отчётов.
    /// </summary>
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    /// <summary>
    /// Конфигурация сущностей отчётной БД.
    /// </summary>
    /// <param name="modelBuilder">Построитель модели EF Core.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ActivityLog>()
            .ToTable("activity_logs");
    }
}

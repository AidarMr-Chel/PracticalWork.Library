using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using System.Reflection.Emit;

namespace PracticalWork.Library.Data.PostgreSql;

/// <summary>
/// Контекст базы данных приложения.
/// Определяет наборы сущностей и конфигурацию моделей,
/// а также автоматически обновляет поле UpdatedAt при изменении сущностей.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Конфигурирует модель базы данных.
    /// Автоматически применяет все конфигурации из текущей сборки.
    /// </summary>
    /// <param name="builder">Построитель модели.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AbstractBookEntity>()
            .ToTable("books") 
            .HasDiscriminator<string>("book_type")  
            .HasValue<AbstractBookEntity>("base")
            .HasValue<ScientificBookEntity>("scientific")
            .HasValue<EducationalBookEntity>("educational")
            .HasValue<FictionBookEntity>("fiction");

        builder.Entity<ReaderEntity>().ToTable("readers");
        builder.Entity<BookBorrowEntity>().ToTable("book_borrows");
        builder.Entity<NotificationLogEntity>().ToTable("notification_logs");
    }

    #region Set UpdateDate on SaveChanges


    /// <summary>
    /// Сохраняет изменения и обновляет поле UpdatedAt у изменённых сущностей.
    /// </summary>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetUpdateDates();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <summary>
    /// Асинхронно сохраняет изменения и обновляет поле UpdatedAt у изменённых сущностей.
    /// </summary>
    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        SetUpdateDates();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Устанавливает текущее время в поле UpdatedAt для всех изменённых сущностей,
    /// реализующих интерфейс <see cref="IEntity"/>.
    /// </summary>
    private void SetUpdateDates()
    {
        var updateDate = DateTime.UtcNow;

        var updatedEntries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in updatedEntries)
        {
            if (entry.Entity is IEntity entity)
                entity.UpdatedAt = updateDate;
        }
    }

    #endregion

    /// <summary>Набор всех книг (базовый тип).</summary>
    internal DbSet<AbstractBookEntity> Books { get; set; }

    /// <summary>Набор учебных пособий.</summary>
    internal DbSet<EducationalBookEntity> EducationalBooks { get; set; }

    /// <summary>Набор художественных книг.</summary>
    internal DbSet<FictionBookEntity> FictionBooks { get; set; }

    /// <summary>Набор научных книг.</summary>
    internal DbSet<ScientificBookEntity> ScientificBooks { get; set; }

    /// <summary>Набор читателей.</summary>
    internal DbSet<ReaderEntity> Readers { get; set; }

    /// <summary>Набор записей о выдаче книг.</summary>
    internal DbSet<BookBorrowEntity> BookBorrows { get; set; }
    public DbSet<NotificationLogEntity> NotificationLogs { get; set; } = null!;
}

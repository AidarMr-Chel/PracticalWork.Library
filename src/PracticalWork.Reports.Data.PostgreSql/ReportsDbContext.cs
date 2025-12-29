using Microsoft.EntityFrameworkCore;
using PracticalWork.Reports.Entities;

namespace PracticalWork.Reports.Data.PostgreSql
{
    /// <summary>
    /// Контекст базы данных для модуля отчётов.
    /// Определяет наборы сущностей и используется для работы с PostgreSQL.
    /// </summary>
    public class ReportsDbContext : DbContext
    {
        public ReportsDbContext(DbContextOptions<ReportsDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Набор логов активности.
        /// </summary>
        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

        /// <summary>
        /// Набор отчётов.
        /// </summary>
        public DbSet<Report> Reports => Set<Report>();
    }
}

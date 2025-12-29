using Microsoft.EntityFrameworkCore;
using PracticalWork.Reports.Entities;
using System.Collections.Generic;

namespace PracticalWork.Reports.Data.PostgreSql
{
    /// <summary>
    /// Контекст базы данных для отчетов
    /// </summary>
    public class ReportsDbContext : DbContext
    {
        public ReportsDbContext(DbContextOptions<ReportsDbContext> options)
            : base(options)
        {
        }

        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
        public DbSet<Report> Reports => Set<Report>();

    }
}

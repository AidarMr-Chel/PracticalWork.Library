using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PracticalWork.Library.Data.PostgreSql.Entities;

namespace PracticalWork.Library.Data.PostgreSql.Configurations;

internal sealed class AbstractBookConfiguration : EntityConfigurationBase<AbstractBookEntity>
{
    public override void Configure(EntityTypeBuilder<AbstractBookEntity> builder)
    {
        base.Configure(builder);

        builder.UseTptMappingStrategy();

        builder.Property(p => p.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.Authors)
            .IsRequired();

        builder.Property(p => p.Year)
            .IsRequired();

        builder.Property(p => p.CoverImagePath)
            .HasMaxLength(500);

        builder.HasMany(c => c.IssuanceRecords)
            .WithOne()
            .HasForeignKey(p => p.BookId);
    }
}
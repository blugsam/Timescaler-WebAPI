using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timescaler.Domain.Entities;

namespace Timescaler.Infrastructure.Persistence.Configurations;

public class ResultEntryConfiguration : IEntityTypeConfiguration<Result>
{
    public void Configure(EntityTypeBuilder<Result> builder)
    {
        builder.ToTable("Results",t =>
        {
            t.HasCheckConstraint("CK_Results_TimeDelta_Positive",
               "\"TimeDelta\" >= INTERVAL '0 seconds'");
        });
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(260);

        builder.HasIndex(x => x.FileName, "IX_Results_FileName_Unique").IsUnique();

        builder.Property(x => x.FirstOperationDate)
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.TimeDelta)
            .HasColumnType("interval");

        var decimalPrecision = 18;
        var decimalScale = 6;
        builder.Property(x => x.AverageValue).HasPrecision(decimalPrecision, decimalScale);
        builder.Property(x => x.MedianValue).HasPrecision(decimalPrecision, decimalScale);
        builder.Property(x => x.MaxValue).HasPrecision(decimalPrecision, decimalScale);
        builder.Property(x => x.MinValue).HasPrecision(decimalPrecision, decimalScale);

        builder.Property(x => x.AverageExecutionTime);

        builder.HasIndex(x => x.FirstOperationDate);
        builder.HasIndex(x => x.AverageValue);
        builder.HasIndex(x => x.AverageExecutionTime);
    }
}

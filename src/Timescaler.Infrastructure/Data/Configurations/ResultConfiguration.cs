using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timescaler.Domain.Entities;

namespace Timescaler.Infrastructure.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<Result>
{
    private const int DecimalPrecision = 18;
    private const int DecimalScale = 6;

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

        builder.Property(x => x.AverageValue).HasPrecision(DecimalPrecision, DecimalScale);
        builder.Property(x => x.MedianValue).HasPrecision(DecimalPrecision, DecimalScale);
        builder.Property(x => x.MaxValue).HasPrecision(DecimalPrecision, DecimalScale);
        builder.Property(x => x.MinValue).HasPrecision(DecimalPrecision, DecimalScale);

        builder.Property(x => x.AverageExecutionTime);

        builder.HasIndex(x => x.FirstOperationDate);
        builder.HasIndex(x => x.AverageValue);
        builder.HasIndex(x => x.AverageExecutionTime);
    }
}

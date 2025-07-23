using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timescaler.Domain.Entities;

namespace Timescaler.Infrastructure.Data.Configurations;

public class ValueConfiguration : IEntityTypeConfiguration<RawValue>
{
    private const int DecimalPrecision = 18;
    private const int DecimalScale = 6;

    public void Configure(EntityTypeBuilder<RawValue> builder)
    {
        builder.ToTable("Values", t =>
        {
            t.HasCheckConstraint("CK_Values_Date_Range",
                "\"Date\" >= '2000-01-01T00:00:00Z'");

            t.HasCheckConstraint("CK_Values_ExecutionTime_Positive",
                "\"ExecutionTime\" >= 0");

            t.HasCheckConstraint("CK_Values_Value_Positive",
                "\"Value\" >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Date)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.ExecutionTime)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasPrecision(DecimalPrecision, DecimalScale)
            .IsRequired();

        builder.HasOne(x => x.Result)
            .WithMany(v => v.RawValues)
            .HasForeignKey(x => x.ResultId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ResultId, x.Date }, "IX_Values_ResultId_Date_Desc")
            .IsDescending(false, true);
    }
}

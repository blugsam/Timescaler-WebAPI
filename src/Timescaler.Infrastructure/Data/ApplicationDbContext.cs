using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Timescaler.Domain.Entities;

namespace Timescaler.Infrastructure.Data;

public class TimescalerDbContext : DbContext
{
    public TimescalerDbContext(DbContextOptions<TimescalerDbContext> options)
        : base(options)
    {
    }

    public DbSet<RawValue> Values { get; set; }
    public DbSet<Result> Results { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}

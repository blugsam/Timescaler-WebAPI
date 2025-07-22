using Microsoft.EntityFrameworkCore;
using Timescaler.Application.Ports;
using Timescaler.Domain.Entities;
using Timescaler.Infrastructure.Data;

namespace Timescaler.Infrastructure.Repositories;

public sealed class RawValueRepository : IRawValueRepository
{
    private readonly TimescalerDbContext _dbContext;

    public RawValueRepository(TimescalerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<RawValue>> GetLastAsync(string fileName, int take = 10, CancellationToken ct = default)
    {
        return await _dbContext.RawValues
            .AsNoTracking()
            .Where(rv => rv.Result.FileName == fileName)
            .OrderByDescending(rv => rv.Date)
            .Take(take)
            .ToListAsync(ct);
    }
}

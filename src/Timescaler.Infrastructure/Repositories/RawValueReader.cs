using Microsoft.EntityFrameworkCore;
using Timescaler.Application.Contracts;
using Timescaler.Application.Ports;
using Timescaler.Infrastructure.Data;

namespace Timescaler.Infrastructure.Repositories;

public class RawValueReader : IRawValueReader
{
    private readonly TimescalerDbContext _dbContext;

    public RawValueReader(TimescalerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<RawValueDto>> GetLastAsync(
        string fileName,
        int take = 10,
        CancellationToken ct = default)
    {
        var resultId = await _dbContext.Results
            .AsNoTracking()
            .Where(r => r.FileName == fileName)
            .Select(r => (Guid?)r.Id)
            .FirstOrDefaultAsync(ct);

        if (resultId is null)
        {
            return Array.Empty<RawValueDto>();
        }

        var rawValues = await _dbContext.Values
            .AsNoTracking()
            .Where(v => v.ResultId == resultId.Value)
            .OrderByDescending(v => v.Date)
            .Take(take)
            .Select(v => new RawValueDto
            {
                Id = v.Id,
                Date = v.Date,
                ExecutionTime = v.ExecutionTime,
                Value = v.Value,
                ResultId = v.ResultId
            })
            .ToListAsync(ct);

        return rawValues;
    }
}
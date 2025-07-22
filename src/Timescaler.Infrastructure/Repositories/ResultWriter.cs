using Microsoft.EntityFrameworkCore;
using Timescaler.Application.Ports;
using Timescaler.Domain.Entities;
using Timescaler.Infrastructure.Data;

namespace Timescaler.Infrastructure.Repositories;

public class ResultWriter : IResultWriter
{
    private readonly TimescalerDbContext _context;

    public ResultWriter(TimescalerDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateOrUpdateAsync(Result result, CancellationToken ct = default)
    {
        var existingResult = await _context.Results
            .FirstOrDefaultAsync(r => r.FileName == result.FileName, ct);

        if (existingResult != null)
        {
            _context.Results.Remove(existingResult);
        }

        await _context.Results.AddAsync(result, ct);

        return result.Id;
    }
}
using Microsoft.EntityFrameworkCore;
using Timescaler.Application.Contracts;
using Timescaler.Application.Ports;
using Timescaler.Domain.Entities;
using Timescaler.Domain.ValueObjects;
using Timescaler.Infrastructure.Data;

namespace Timescaler.Infrastructure.Repositories;

public sealed class ResultRepository : IResultRepository
{
    private readonly TimescalerDbContext _dbContext;

    public ResultRepository(TimescalerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpsertAsync(string fileName, IReadOnlyList<RawDataPoint> records, CancellationToken ct)
    {
        var existingResult = await _dbContext.Results
            .FirstOrDefaultAsync(r => r.FileName == fileName, ct);

        if (existingResult != null)
        {
            _dbContext.Results.Remove(existingResult);
        }

        var newResult = Result.Create(fileName, records);

        await _dbContext.Results.AddAsync(newResult, ct);

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<(IReadOnlyList<Result> Items, int TotalCount)> FindAsync(ResultFilter filter, Page page, CancellationToken ct)
    {
        var query = _dbContext.Results.AsNoTracking();

        query = ApplyFilter(query, filter);

        var countTask = query.CountAsync(ct);
        var itemsTask = query
            .OrderByDescending(r => r.FirstOperationDate)
            .Skip((page.Number - 1) * page.Size)
            .Take(page.Size)
            .ToListAsync(ct);

        await Task.WhenAll(countTask, itemsTask);

        return (itemsTask.Result, countTask.Result);
    }

    private IQueryable<Result> ApplyFilter(IQueryable<Result> query, ResultFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.FileName))
        {
            query = query.Where(r => EF.Functions.ILike(r.FileName, $"%{filter.FileName}%"));
        }
        if (filter.FirstOperationDateRange is { } dateRange)
        {
            query = query.Where(r => r.FirstOperationDate >= dateRange.From && r.FirstOperationDate <= dateRange.To);
        }
        if (filter.AverageValueRange is { } valueRange)
        {
            query = query.Where(r => r.AverageValue >= valueRange.Min && r.AverageValue <= valueRange.Max);
        }
        if (filter.AverageExecutionTimeRange is { } timeRange)
        {
            query = query.Where(r => r.AverageExecutionTime >= timeRange.Min && r.AverageExecutionTime <= timeRange.Max);
        }
        return query;
    }
}
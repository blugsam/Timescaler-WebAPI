using Microsoft.EntityFrameworkCore;
using Timescaler.Application.Contracts;
using Timescaler.Application.Services.Interfaces;
using Timescaler.Infrastructure.Data;

namespace Timescaler.Infrastructure.Repositories;

public class ResultReader : IResultReader
{
    private readonly TimescalerDbContext _dbContext;

    public ResultReader(TimescalerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedListDto<ResultDto>> FindAsync(ResultFilterDto filter, PageRequestDto page, CancellationToken ct)
    {
        var query = _dbContext.Results.AsNoTracking();

        if (!string.IsNullOrEmpty(filter.FileName))
            query = query.Where(r => EF.Functions.ILike(r.FileName, $"%{filter.FileName}%"));

        if (filter.StartDateFrom.HasValue)
            query = query.Where(r => r.FirstOperationDate >= filter.StartDateFrom.Value);
        if (filter.StartDateTo.HasValue)
            query = query.Where(r => r.FirstOperationDate <= filter.StartDateTo.Value);

        if (filter.AvgValueMin is not null)
            query = query.Where(r => r.AverageValue >= filter.AvgValueMin.Value);
        if (filter.AvgValueMax is not null)
            query = query.Where(r => r.AverageValue <= filter.AvgValueMax.Value);

        if (filter.AvgExecTimeMin is not null)
            query = query.Where(r => r.AverageExecutionTime >= filter.AvgExecTimeMin.Value);
        if (filter.AvgExecTimeMax is not null)
            query = query.Where(r => r.AverageExecutionTime <= filter.AvgExecTimeMax.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(r => r.FirstOperationDate)
            .Skip((page.PageNumber - 1) * page.PageSize)
            .Take(page.PageSize)
            .Select(r => new ResultDto
            {
                Id = r.Id,
                FileName = r.FileName,
                FirstOperationDate = r.FirstOperationDate,
                TimeDelta = r.TimeDelta,
                AverageExecutionTime = r.AverageExecutionTime,
                AverageValue = r.AverageValue,
                MedianValue = r.MedianValue,
                MaxValue = r.MaxValue,
                MinValue = r.MinValue
            })
            .ToListAsync(ct);

        return new PagedListDto<ResultDto>(items, totalCount, page.PageNumber, page.PageSize);
    }
}
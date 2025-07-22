using Timescaler.Domain.Entities;
using Timescaler.Domain.ValueObjects;

namespace Timescaler.Application.Ports;

public interface IResultRepository
{
    Task UpsertAsync(string fileName, IReadOnlyList<RawDataPoint> records, CancellationToken ct = default);

    Task<(IReadOnlyList<Result> Items, int TotalCount)> FindAsync(ResultFilter filter, Page page, CancellationToken ct = default);
}
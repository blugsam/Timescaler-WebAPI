using Timescaler.Application.Contracts;
using Timescaler.Domain.ValueObjects;

namespace Timescaler.Application.Services.Interfaces;

public interface IDataProcessingService
{
    Task ProcessCsvFileAsync(string fileName, Stream fileStream, CancellationToken ct);
    Task<PaginatedList<ResultDto>> GetResultsAsync(ResultFilter filter, Page page, CancellationToken ct);
    Task<IReadOnlyList<RawValueDto>> GetLastRawValuesAsync(string fileName, CancellationToken ct);
}

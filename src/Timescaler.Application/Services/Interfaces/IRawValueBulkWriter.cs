using Timescaler.Application.Contracts;

namespace Timescaler.Application.Services.Interfaces;

public interface IRawValueBulkWriter
{
    Task BulkInsertAsync(
    IReadOnlyCollection<RawValueDto> values,
    CancellationToken ct = default);
}

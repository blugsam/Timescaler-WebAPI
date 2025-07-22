using Timescaler.Application.Contracts;

namespace Timescaler.Application.Ports;

public interface IRawValueBulkWriter
{
    Task BulkInsertAsync(
    IReadOnlyCollection<RawValueDto> values,
    CancellationToken ct = default);
}

using Timescaler.Application.Contracts;

namespace Timescaler.Application.Ports;

public interface IRawValueReader
{
    Task<IReadOnlyList<RawValueDto>> GetLastAsync(
    string fileName,
    int take = 10,
    CancellationToken ct = default);
}

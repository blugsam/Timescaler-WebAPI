using Timescaler.Application.Contracts;

namespace Timescaler.Application.Services.Interfaces;

public interface IRawValueReader
{
    Task<IReadOnlyList<RawValueDto>> GetLastAsync(
    string fileName,
    int take = 10,
    CancellationToken ct = default);
}

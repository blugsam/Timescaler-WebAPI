using Timescaler.Domain.Entities;

namespace Timescaler.Application.Ports;

public interface IRawValueRepository
{
    Task<IReadOnlyList<RawValue>> GetLastAsync(string fileName, int take = 10, CancellationToken ct = default);
}

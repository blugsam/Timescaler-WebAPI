using Timescaler.Domain.Entities;

namespace Timescaler.Domain.Repositories;

public interface IRawValueRepository
{
    Task<IReadOnlyList<RawValue>> GetLastAsync(string fileName, int take = 10, CancellationToken ct = default);
}

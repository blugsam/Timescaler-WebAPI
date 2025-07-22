using Timescaler.Domain.Entities;

namespace Timescaler.Application.Ports;

public interface IResultWriter
{
    Task<Guid> CreateOrUpdateAsync(Result result, CancellationToken ct = default);
}
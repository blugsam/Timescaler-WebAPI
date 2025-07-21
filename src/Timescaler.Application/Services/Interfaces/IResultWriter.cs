using Timescaler.Domain.Entities;

namespace Timescaler.Application.Services.Interfaces;

public interface IResultWriter
{
    Task<Guid> CreateOrUpdateAsync(Result result, CancellationToken ct = default);
}
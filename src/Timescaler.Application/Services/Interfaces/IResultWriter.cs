using Timescaler.Application.Contracts;

namespace Timescaler.Application.Services.Interfaces;

public interface IResultWriter
{
    Task<ImportSummaryDto> ImportAsync(
    Stream csvStream,
    string fileName,
    CancellationToken ct = default);
}

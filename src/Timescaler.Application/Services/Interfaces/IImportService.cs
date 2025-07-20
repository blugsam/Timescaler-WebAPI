using Timescaler.Application.Contracts;

namespace Timescaler.Application.Services.Interfaces;

public interface IImportService
{
    Task<ImportSummaryDto> ImportCsvAsync(
    Stream csv,
    string fileName,
    CancellationToken ct = default);
}

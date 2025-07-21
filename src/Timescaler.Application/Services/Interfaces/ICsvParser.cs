namespace Timescaler.Application.Services.Interfaces;

public interface ICsvParser
{
    Task<CsvProcessingResult> ParseAndValidateAsync(Stream stream, CancellationToken ct = default);
}
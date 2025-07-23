namespace Timescaler.Application.Services.Interfaces;

public interface ICsvParser
{
    IAsyncEnumerable<(int LineNumber, string[] Fields)> ParseAsync(Stream stream, CancellationToken ct = default);
}
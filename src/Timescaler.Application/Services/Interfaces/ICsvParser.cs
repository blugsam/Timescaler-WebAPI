namespace Timescaler.Application.Services.Interfaces;

public interface ICsvParser
{
    IAsyncEnumerable<(int LineNumber, string[] Fields)> ParseAsync(Stream stream, char separator = ';', CancellationToken ct = default);
}
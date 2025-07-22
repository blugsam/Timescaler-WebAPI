using System.Runtime.CompilerServices;
using Timescaler.Application.Services.Interfaces;

namespace Timescaler.Application.Services.Parsing;

public class CsvParser : ICsvParser
{
    public async IAsyncEnumerable<(int LineNumber, string[] Fields)> ParseAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        using var reader = new StreamReader(stream);

        await reader.ReadLineAsync(ct);

        string? line;
        int lineNumber = 1;
        while ((line = await reader.ReadLineAsync(ct)) != null)
        {
            lineNumber++;
            var fields = line.Split(';');
            yield return (lineNumber, fields);
        }
    }
}
using Timescaler.Application.Validation;

namespace Timescaler.Application.Services.Interfaces;

public interface ICsvRowParser
{
    bool TryParse(RawCsvRow rawRow, out ParsedCsvRow? parsedRow, out string? parseError);
}

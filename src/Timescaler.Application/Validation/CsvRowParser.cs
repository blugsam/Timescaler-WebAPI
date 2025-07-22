using System.Globalization;
using Timescaler.Application.Services.Interfaces;

namespace Timescaler.Application.Validation;

public class CsvRowParser : ICsvRowParser
{
    public bool TryParse(RawCsvRow rawRow, out ParsedCsvRow? parsedRow, out string? parseError)
    {
        parsedRow = null;
        parseError = null;

        var fields = rawRow.Fields;

        if (!DateTime.TryParse(fields[0], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var date))
        {
            parseError = "Неверный формат даты.";
            return false;
        }

        if (!double.TryParse(fields[1], CultureInfo.InvariantCulture, out var executionTime))
        {
            parseError = "Неверный формат времени выполнения.";
            return false;
        }

        if (!decimal.TryParse(fields[2], CultureInfo.InvariantCulture, out var value))
        {
            parseError = "Неверный формат значения.";
            return false;
        }

        parsedRow = new ParsedCsvRow(rawRow.LineNumber, date, executionTime, value);
        return true;
    }
}

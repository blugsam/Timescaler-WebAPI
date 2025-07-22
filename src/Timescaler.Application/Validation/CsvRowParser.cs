using System.Globalization;
using Timescaler.Application.Services.Interfaces;

namespace Timescaler.Application.Validation;

public class CsvRowParser : ICsvRowParser
{
    private const string CustomDateTimeFormat = "yyyy-MM-ddTHH-mm-ss.ffffZ";

    public bool TryParse(RawCsvRow rawRow, out ParsedCsvRow? parsedRow, out string? parseError)
    {
        parsedRow = null;
        parseError = null;

        var fields = rawRow.Fields;

        //if (!DateTime.TryParse(fields[0], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var date))
        //{
        //    parseError = "Неверный формат даты.";
        //    return false;
        //}

        if (!DateTime.TryParseExact(fields[0], CustomDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var date))
        {
            parseError = "Неверный формат даты. Ожидался формат 'yyyy-MM-ddTHH-mm-ss.ffffZ'.";
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

using System.Globalization;
using Timescaler.Application.Services.Interfaces;

namespace Timescaler.Application.Services;

public record ParsedValueRecord(DateTime Date, double ExecutionTime, decimal Value);

public record CsvProcessingResult(List<ParsedValueRecord> Records, string? ErrorMessage = null)
{
    public bool IsSuccess => ErrorMessage is null;
}

public class CsvParser : ICsvParser
{
    private const int MaxRowCount = 10000;
    private static readonly DateTime MinDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public async Task<CsvProcessingResult> ParseAndValidateAsync(Stream stream, CancellationToken ct = default)
    {
        var records = new List<ParsedValueRecord>();

        using var reader = new StreamReader(stream);

        await reader.ReadLineAsync(ct);

        string? line;
        int lineNumber = 1;
        while ((line = await reader.ReadLineAsync(ct)) != null)
        {
            lineNumber++;
            if (records.Count >= MaxRowCount)
            {
                return new(records, $"Ошибка валидации: количество строк превышает максимальное ({MaxRowCount}).");
            }

            var parts = line.Split(';');
            if (parts.Length != 3)
            {
                return new(records, $"Ошибка валидации в строке {lineNumber}: неверное количество столбцов.");
            }

            if (!DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var date))
            {
                return new(records, $"Ошибка валидации в строке {lineNumber}: неверный формат даты.");
            }

            if (!double.TryParse(parts[1], CultureInfo.InvariantCulture, out var executionTime))
            {
                return new(records, $"Ошибка валидации в строке {lineNumber}: неверный формат времени выполнения.");
            }

            if (!decimal.TryParse(parts[2], CultureInfo.InvariantCulture, out var value))
            {
                return new(records, $"Ошибка валидации в строке {lineNumber}: неверный формат значения.");
            }

            var now = DateTime.UtcNow;
            if (date > now || date < MinDate)
            {
                return new(records, $"Ошибка валидации в строке {lineNumber}: дата выходит за допустимый диапазон.");
            }
            if (executionTime < 0)
            {
                return new(records, $"Ошибка валидации в строке {lineNumber}: время выполнения не может быть отрицательным.");
            }
            if (value < 0)
            {
                return new(records, $"Ошибка валидации в строке {lineNumber}: значение не может быть отрицательным.");
            }

            records.Add(new ParsedValueRecord(date, executionTime, value));
        }

        if (records.Count == 0)
        {
            return new(records, "Ошибка валидации: файл не содержит данных.");
        }

        return new(records, null);
    }
}

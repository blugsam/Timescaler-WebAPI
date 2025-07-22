using System.Globalization;
using Timescaler.Application.Contracts;

namespace Timescaler.Application.Validation;

public class FileValidator : IFileValidator
{
    private const int MaxRowCount = 10000;
    private static readonly DateTime MinDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public async Task<FileValidationResult> ValidateAsync(
        IAsyncEnumerable<(int LineNumber, string[] Fields)> parsedLines,
        CancellationToken ct = default)
    {
        var validRecords = new List<ParsedValueRecord>();
        var errors = new List<ValidationError>();

        await foreach (var (lineNumber, fields) in parsedLines.WithCancellation(ct))
        {
            var lineErrors = new List<ValidationError>();
            var parsedRecord = ValidateLine(lineNumber, fields, lineErrors);

            if (lineErrors.Any())
            {
                errors.AddRange(lineErrors);
            }
            else
            {
                validRecords.Add(parsedRecord!);
            }
        }

        if (!errors.Any())
        {
            if (validRecords.Count == 0)
                errors.Add(new ValidationError(0, "Файл не содержит данных."));
            if (validRecords.Count > MaxRowCount)
                errors.Add(new ValidationError(0, $"Количество строк ({validRecords.Count}) превышает максимальное ({MaxRowCount})."));
        }

        return new FileValidationResult
        {
            ValidRecords = validRecords,
            Errors = errors
        };
    }

    public static ParsedValueRecord? ValidateLine(int lineNumber, string[] fields, List<ValidationError> errors)
    {
        if (fields.Length != 3)
        {
            errors.Add(new ValidationError(lineNumber, "Неверное количество столбцов (ожидается 3)."));
            return null;
        }

        var dateParseSuccess = DateTime.TryParse(fields[0], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var date);
        var timeParseSuccess = double.TryParse(fields[1], CultureInfo.InvariantCulture, out var executionTime);
        var valueParseSuccess = decimal.TryParse(fields[2], CultureInfo.InvariantCulture, out var value);

        if (!dateParseSuccess) errors.Add(new ValidationError(lineNumber, "Неверный формат даты."));
        if (!timeParseSuccess) errors.Add(new ValidationError(lineNumber, "Неверный формат времени выполнения."));
        if (!valueParseSuccess) errors.Add(new ValidationError(lineNumber, "Неверный формат значения."));

        if (errors.Any()) return null;

        if (date > DateTime.UtcNow || date < MinDate) errors.Add(new ValidationError(lineNumber, "Дата выходит за допустимый диапазон."));
        if (executionTime < 0) errors.Add(new ValidationError(lineNumber, "Время выполнения не может быть отрицательным."));
        if (value < 0) errors.Add(new ValidationError(lineNumber, "Значение не может быть отрицательным."));

        if (errors.Any()) return null;

        return new ParsedValueRecord(date, executionTime, value);
    }
}
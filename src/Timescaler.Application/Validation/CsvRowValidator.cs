using FluentValidation;
using System.Globalization;
using Timescaler.Application.Contracts;

namespace Timescaler.Application.Validation;

public class CsvRowValidator : AbstractValidator<CsvRow>
{
    private const int ExpectedFieldCount = 3;

    public CsvRowValidator()
    {
        RuleFor(x => x.Fields)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Строка не может быть null.")
            .Must(fields => fields.Length == ExpectedFieldCount)
            .WithMessage($"Строка {ExpectedFieldCount} должна содержать ровно {ExpectedFieldCount} поля.");

        RuleFor(x => x)
            .Custom((row, context) =>
            {
                var (isParsed, record) = TryParseToRecord(row.Fields);

                if (!isParsed)
                {
                    context.AddFailure("Одно или несколько полей имеют неверный формат или пусты.");
                    return;
                }

                if (record!.Date > DateTime.UtcNow)
                    context.AddFailure("Date", "Дата не может быть в будущем.");

                if (record.Date < new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                    context.AddFailure("Date", "Дата не может быть ранее 01.01.2000.");

                if (record.ExecutionTime < 0)
                    context.AddFailure("ExecutionTime", "Время выполнения не может быть отрицательным.");

                if (record.Value < 0)
                    context.AddFailure("Value", "Значение показателя не может быть отрицательным.");
            });
    }

    public static (bool Success, ParsedValueRecord? Record) TryParseToRecord(string[] fields)
    {
        if (fields.Length != ExpectedFieldCount ||
            !DateTime.TryParse(fields[0], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var date) ||
            !double.TryParse(fields[1], CultureInfo.InvariantCulture, out var executionTime) ||
            !decimal.TryParse(fields[2], CultureInfo.InvariantCulture, out var value))
        {
            return (false, null);
        }

        return (true, new ParsedValueRecord(date, executionTime, value));
    }
}

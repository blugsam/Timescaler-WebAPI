using FluentValidation;

namespace Timescaler.Application.Validation;

public class CsvBusinessRulesValidator : AbstractValidator<ParsedCsvRow>
{
    private static readonly DateTime MinDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public CsvBusinessRulesValidator()
    {
        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Дата не может быть в будущем.")
            .GreaterThanOrEqualTo(MinDate)
            .WithMessage("Дата не может быть ранее 01.01.2000.");

        RuleFor(x => x.ExecutionTime)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Время выполнения не может быть отрицательным.");

        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Значение показателя не может быть отрицательным.");
    }
}

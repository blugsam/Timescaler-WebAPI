using FluentValidation;

namespace Timescaler.Application.Validation;

public class CsvBusinessRulesValidator : AbstractValidator<ParsedCsvRow>
{
    private static readonly DateTime MinDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public CsvBusinessRulesValidator()
    {
        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("The date cannot be in the future.")
            .GreaterThanOrEqualTo(MinDate)
            .WithMessage("The date cannot be earlier than 01.01.2000.");

        RuleFor(x => x.ExecutionTime)
            .GreaterThanOrEqualTo(0)
            .WithMessage("The execution time cannot be negative.");

        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0)
            .WithMessage("The indicator value cannot be negative.");
    }
}

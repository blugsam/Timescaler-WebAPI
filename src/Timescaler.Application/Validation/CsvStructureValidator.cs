using FluentValidation;

namespace Timescaler.Application.Validation;

public class CsvStructureValidator : AbstractValidator<RawCsvRow>
{
    private const int ExpectedFieldCount = 3;

    public CsvStructureValidator()
    {
        RuleFor(x => x.Fields)
            .NotNull().WithMessage("The string cannot be null.");

        RuleFor(x => x.Fields.Length)
            .Equal(ExpectedFieldCount)
            .WithMessage($"The string must contain exactly {ExpectedFieldCount} fields.");

        When(x => x.Fields.Length == ExpectedFieldCount, () =>
        {
            RuleFor(x => x.Fields[0]).NotEmpty().WithMessage("The 'Date' field cannot be empty.");
            RuleFor(x => x.Fields[1]).NotEmpty().WithMessage("The 'Execution Time' field cannot be empty.");
            RuleFor(x => x.Fields[2]).NotEmpty().WithMessage("The 'Value' field cannot be empty.");
        });
    }
}

using FluentValidation;

namespace Timescaler.Application.Validation;

public class CsvStructureValidator : AbstractValidator<RawCsvRow>
{
    private const int ExpectedFieldCount = 3;

    public CsvStructureValidator()
    {
        RuleFor(x => x.Fields)
            .NotNull().WithMessage("Строка не может быть null.");

        RuleFor(x => x.Fields.Length)
            .Equal(ExpectedFieldCount)
            .WithMessage($"Строка должна содержать ровно {ExpectedFieldCount} поля.");

        When(x => x.Fields.Length == ExpectedFieldCount, () =>
        {
            RuleFor(x => x.Fields[0]).NotEmpty().WithMessage("Поле 'Date' не может быть пустым.");
            RuleFor(x => x.Fields[1]).NotEmpty().WithMessage("Поле 'ExecutionTime' не может быть пустым.");
            RuleFor(x => x.Fields[2]).NotEmpty().WithMessage("Поле 'Value' не может быть пустым.");
        });
    }
}

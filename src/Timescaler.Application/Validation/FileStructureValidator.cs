using FluentValidation;

namespace Timescaler.Application.Validation;

public class FileStructureValidator : AbstractValidator<FileStructure>
{
    private const int MaxRowCount = 10000;
    private const int MinRowCount = 1;

    public FileStructureValidator()
    {
        RuleFor(x => x.RowCount)
            .GreaterThanOrEqualTo(MinRowCount)
            .WithMessage($"The file must contain at least a {MinRowCount} data string.")
            .LessThanOrEqualTo(MaxRowCount)
            .WithMessage($"The number of lines in the file ({nameof(FileStructure.RowCount)}) exceeds the maximum ({MaxRowCount}).");
    }
}

public record FileStructure(int RowCount);
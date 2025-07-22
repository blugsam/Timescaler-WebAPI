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
            .WithMessage($"Файл должен содержать минимум {MinRowCount} строку с данными.")
            .LessThanOrEqualTo(MaxRowCount)
            .WithMessage($"Количество строк в файле ({nameof(FileStructure.RowCount)}) превышает максимум ({MaxRowCount}).");
    }
}

public record FileStructure(int RowCount);
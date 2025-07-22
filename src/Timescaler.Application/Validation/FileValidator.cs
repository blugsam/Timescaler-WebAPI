using FluentValidation;
using Timescaler.Application.Contracts;
using Timescaler.Application.Services.Interfaces;

namespace Timescaler.Application.Validation;

public class FileValidator : IFileValidator
{
    private const int MaxRowCount = 10000;
    private const int MinRowCount = 1;

    private readonly IValidator<CsvRow> _csvRowValidator;

    public FileValidator(IValidator<CsvRow> csvRowValidator)
    {
        _csvRowValidator = csvRowValidator;
    }

    public async Task<FileValidationResult> ValidateAsync(
        IAsyncEnumerable<(int LineNumber, string[] Fields)> parsedLines,
        CancellationToken ct = default)
    {
        var validRecords = new List<ParsedValueRecord>();
        var errors = new List<ValidationError>();
        var rowCount = 0;

        await foreach (var (lineNumber, fields) in parsedLines.WithCancellation(ct))
        {
            rowCount++;
            var csvRow = new CsvRow(lineNumber, fields);
            var validationResult = await _csvRowValidator.ValidateAsync(csvRow, ct);

            if (validationResult.IsValid)
            {
                var (_, record) = CsvRowValidator.TryParseToRecord(fields);
                validRecords.Add(record!);
            }
            else
            {
                foreach (var error in validationResult.Errors)
                {
                    errors.Add(new ValidationError(lineNumber, error.ErrorMessage));
                }
            }
        }

        if (rowCount < MinRowCount)
            errors.Insert(0, new ValidationError(0, $"Файл должен содержать минимум {MinRowCount} строку с данными."));
        if (rowCount > MaxRowCount)
            errors.Insert(0, new ValidationError(0, $"Количество строк в файле ({rowCount}) превышает максимум ({MaxRowCount})."));

        return new FileValidationResult
        {
            ValidRecords = errors.Any() ? new List<ParsedValueRecord>() : validRecords,
            Errors = errors
        };
    }
}

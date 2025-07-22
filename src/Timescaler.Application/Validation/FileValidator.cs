using FluentValidation;
using Timescaler.Application.Contracts;
using Timescaler.Application.Services.Interfaces;

namespace Timescaler.Application.Validation;

public class FileValidator : IFileValidator
{
    private readonly IValidator<RawCsvRow> _structureValidator;
    private readonly ICsvRowParser _parser;
    private readonly IValidator<ParsedCsvRow> _businessRulesValidator;
    private readonly IValidator<FileStructure> _fileStructureValidator;

    public FileValidator(
        IValidator<RawCsvRow> structureValidator,
        ICsvRowParser parser,
        IValidator<ParsedCsvRow> businessRulesValidator,
        IValidator<FileStructure> fileStructureValidator)
    {
        _structureValidator = structureValidator;
        _parser = parser;
        _businessRulesValidator = businessRulesValidator;
        _fileStructureValidator = fileStructureValidator;
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
            var rawRow = new RawCsvRow(lineNumber, fields);

            // Этап 1: Структурная валидация
            var structureResult = await _structureValidator.ValidateAsync(rawRow, ct);
            if (!structureResult.IsValid)
            {
                errors.AddRange(structureResult.Errors.Select(e =>
                    new ValidationError(lineNumber, e.ErrorMessage)));
                continue;
            }

            // Этап 2: Парсинг
            if (!_parser.TryParse(rawRow, out var parsedRow, out var parseError))
            {
                errors.Add(new ValidationError(lineNumber, parseError!));
                continue;
            }

            // Этап 3: Бизнес-валидация
            var businessResult = await _businessRulesValidator.ValidateAsync(parsedRow!, ct);
            if (!businessResult.IsValid)
            {
                errors.AddRange(businessResult.Errors.Select(e =>
                    new ValidationError(lineNumber, e.ErrorMessage)));
                continue;
            }

            // Все этапы пройдены успешно
            validRecords.Add(new ParsedValueRecord(
                parsedRow.Date,
                parsedRow.ExecutionTime,
                parsedRow.Value));
        }

        // Этап 4: Валидация файла как целого
        var fileStructure = new FileStructure(rowCount);
        var fileResult = await _fileStructureValidator.ValidateAsync(fileStructure, ct);
        if (!fileResult.IsValid)
        {
            errors.InsertRange(0, fileResult.Errors.Select(e =>
                new ValidationError(0, e.ErrorMessage)));
        }

        return new FileValidationResult
        {
            ValidRecords = errors.Any() ? new List<ParsedValueRecord>() : validRecords,
            Errors = errors
        };
    }
}

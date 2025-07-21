using Microsoft.Extensions.Logging;
using Timescaler.Application.Contracts;
using Timescaler.Application.Services.Interfaces;
using Timescaler.Domain.Entities;

namespace Timescaler.Application.Services;

public class ImportService : IImportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IResultWriter _resultWriter;
    private readonly ICsvParser _csvParser;
    private readonly ILogger<ImportService> _logger;

    public ImportService(
        IUnitOfWork unitOfWork,
        IResultWriter resultWriter,
        ICsvParser csvProcessor,
        ILogger<ImportService> logger)
    {
        _unitOfWork = unitOfWork;
        _resultWriter = resultWriter;
        _csvParser = csvProcessor;
        _logger = logger;
    }

    public async Task<ImportSummaryDto> ImportCsvAsync(Stream csv, string fileName, CancellationToken ct = default)
    {
        try
        {
            var processingResult = await _csvParser.ParseAndValidateAsync(csv, ct);
            if (!processingResult.IsSuccess)
            {
                return new ImportSummaryDto(false, fileName, 0, null, processingResult.ErrorMessage);
            }

            var records = processingResult.Records;

            var resultEntity = CreateResultFromRecords(records, fileName);

            await _unitOfWork.BeginTransactionAsync(ct);

            var resultId = await _resultWriter.CreateOrUpdateAsync(resultEntity, ct);

            await _unitOfWork.CommitAsync(ct);

            _logger.LogInformation("Файл {FileName} успешно импортирован. Записей: {RowCount}. ResultId: {ResultId}",
                fileName, records.Count, resultId);

            return new ImportSummaryDto(true, fileName, records.Count, resultId);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(ct);
            _logger.LogError(ex, "Произошла ошибка при импорте файла {FileName}", fileName);
            return new ImportSummaryDto(false, fileName, 0, null, $"Внутренняя ошибка сервера: {ex.Message}");
        }
    }

    private Result CreateResultFromRecords(IReadOnlyList<ParsedValueRecord> records, string fileName)
    {
        var sortedValues = records.Select(r => r.Value).OrderBy(v => v).ToList();
        var dates = records.Select(r => r.Date).ToList();

        var minDate = dates.Min();
        var maxDate = dates.Max();
        var timeDelta = maxDate - minDate;

        var medianValue = CalculateMedian(sortedValues);

        var result = new Result
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            FirstOperationDate = minDate,
            TimeDelta = timeDelta,
            AverageExecutionTime = records.Average(r => r.ExecutionTime),
            AverageValue = records.Average(r => r.Value),
            MedianValue = medianValue,
            MaxValue = sortedValues.Last(),
            MinValue = sortedValues.First(),
            RawValues = new List<RawValue>()
        };

        foreach (var record in records)
        {
            result.RawValues.Add(new RawValue
            {
                Id = Guid.NewGuid(),
                Date = record.Date,
                ExecutionTime = record.ExecutionTime,
                Value = record.Value,
                Result = result
            });
        }

        return result;
    }

    private decimal CalculateMedian(IReadOnlyList<decimal> sortedValues)
    {
        var count = sortedValues.Count;
        var mid = count / 2;
        if (count % 2 == 0)
        {
            return (sortedValues[mid - 1] + sortedValues[mid]) / 2;
        }
        return sortedValues[mid];
    }
}
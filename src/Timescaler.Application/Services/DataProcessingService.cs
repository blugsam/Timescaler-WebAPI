using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Timescaler.Application.Contracts;
using Timescaler.Application.Mapping;
using Timescaler.Domain.Repositories;
using Timescaler.Application.Services.Interfaces;
using Timescaler.Domain.ValueObjects;

namespace Timescaler.Application.Services;

public class DataProcessingService : IDataProcessingService
{
    private readonly IResultRepository _resultRepository;
    private readonly IRawValueRepository _rawValueRepository;
    private readonly ICsvParser _csvParser;
    private readonly IFileValidator _fileValidator;
    private readonly ILogger<DataProcessingService> _logger;

    public DataProcessingService(
        IResultRepository resultRepository,
        IRawValueRepository rawValueRepository,
        ICsvParser csvParser,
        IFileValidator fileValidator,
        ILogger<DataProcessingService> logger)
    {
        _resultRepository = resultRepository;
        _rawValueRepository = rawValueRepository;
        _csvParser = csvParser;
        _fileValidator = fileValidator;
        _logger = logger;
    }

    public async Task ProcessCsvFileAsync(string fileName, Stream fileStream, CancellationToken ct)
    {
        _logger.LogInformation("Начало обработки файла: {FileName}", fileName);

        var lines = _csvParser.ParseAsync(fileStream, ct);

        var validationResult = await _fileValidator.ValidateAsync(lines, ct);

        if (!validationResult.IsSuccess)
        {
            _logger.LogWarning("Файл {FileName} не прошел валидацию. Ошибок: {ErrorCount}",
                fileName, validationResult.Errors.Count);

            var failures = validationResult.Errors
                .Select(e => new ValidationFailure(
                    propertyName: e.LineNumber > 0 ? $"Line[{e.LineNumber}]" : "File",
                    errorMessage: e.Message))
                .ToList();

            throw new ValidationException(failures);
        }

        _logger.LogInformation("Файл {FileName} прошел валидацию. Начинаем сохранение.", fileName);

        var dataPoints = validationResult.ValidRecords
            .Select(r => new RawDataPoint(r.Date, r.ExecutionTime, r.Value))
            .ToList();

        await _resultRepository.UpsertAsync(fileName, dataPoints, ct);

        _logger.LogInformation("Файл {FileName} успешно обработан. Записей: {Count}", fileName, dataPoints.Count);
    }

    public async Task<PaginatedList<ResultDto>> GetResultsAsync(ResultFilter filter, Page page, CancellationToken ct)
    {
        var (items, totalCount) = await _resultRepository.FindAsync(filter, page, ct);

        var dtos = items.Select(MappingExtensions.ToDto).ToList();

        return new PaginatedList<ResultDto>(dtos, page.Number, page.Size, totalCount);
    }

    public async Task<IReadOnlyList<RawValueDto>> GetLastRawValuesAsync(string fileName, CancellationToken ct)
    {
        const int valuesToTake = 10;
        var values = await _rawValueRepository.GetLastAsync(fileName, valuesToTake, ct);

        return values.Select(MappingExtensions.ToDto).ToList();
    }
}

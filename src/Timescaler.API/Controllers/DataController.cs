using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Timescaler.Application.Contracts;
using Timescaler.Application.Services.Interfaces;
using Timescaler.Domain.ValueObjects;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly IDataProcessingService _dataProcessingService;
    private readonly ILogger<DataController> _logger;

    public DataController(IDataProcessingService dataProcessingService, ILogger<DataController> logger)
    {
        _dataProcessingService = dataProcessingService;
        _logger = logger;
    }

    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Файл не был предоставлен.");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            await _dataProcessingService.ProcessCsvFileAsync(file.FileName, stream, ct);
            return Ok($"Файл '{file.FileName}' успешно обработан.");
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Файл {FileName} не прошел валидацию. Ошибки: {Errors}",
                file.FileName,
                string.Join("; ", ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")));

            return BadRequest(new
            {
                Title = "Ошибки валидации файла",
                Errors = ex.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Непредвиденная ошибка при обработке файла {FileName}", file.FileName);
            return StatusCode(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера.");
        }
    }

    [HttpGet("results")]
    [ProducesResponseType(typeof(PaginatedList<ResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResults(
        [FromQuery] string? fileName,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] decimal? minAvgValue,
        [FromQuery] decimal? maxAvgValue,
        [FromQuery] double? minAvgExecTime,
        [FromQuery] double? maxAvgExecTime,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var filter = new ResultFilter(
            FileName: fileName,
            FirstOperationDateRange: (fromDate.HasValue && toDate.HasValue) ? new DateRange(fromDate.Value, toDate.Value) : null,
            AverageValueRange: (minAvgValue.HasValue && maxAvgValue.HasValue) ? new ValueRange(minAvgValue.Value, maxAvgValue.Value) : null,
            AverageExecutionTimeRange: (minAvgExecTime.HasValue && maxAvgExecTime.HasValue) ? new ExecutionTimeRange(minAvgExecTime.Value, maxAvgExecTime.Value) : null
        );
        var page = new Page(pageNumber, pageSize);

        var results = await _dataProcessingService.GetResultsAsync(filter, page, ct);
        return Ok(results);
    }

    [HttpGet("{fileName}/last-values")]
    [ProducesResponseType(typeof(IReadOnlyList<RawValueDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLastValues(string fileName, CancellationToken ct)
    {
        var values = await _dataProcessingService.GetLastRawValuesAsync(fileName, ct);

        if (!values.Any())
        {
            return NotFound($"Данные для файла '{fileName}' не найдены.");
        }

        return Ok(values);
    }
}
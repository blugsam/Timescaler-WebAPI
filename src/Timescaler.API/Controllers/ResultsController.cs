using Microsoft.AspNetCore.Mvc;
using Timescaler.Application.Contracts;
using Timescaler.Application.Services.Interfaces;
using Timescaler.Domain.ValueObjects;

[ApiController]
[Route("api/results")]
public class ResultsController : ControllerBase
{
    private readonly IDataProcessingService _dataProcessingService;

    public ResultsController(IDataProcessingService dataProcessingService)
    {
        _dataProcessingService = dataProcessingService;
    }

    [HttpGet]
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
            return NotFound($"No data was found for the file '{fileName}'.");
        }

        return Ok(values);
    }
}
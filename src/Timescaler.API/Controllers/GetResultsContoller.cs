using Microsoft.AspNetCore.Mvc;
using Timescaler.Application.Contracts;
using Timescaler.Application.Services.Interfaces;
using Timescaler.Domain.ValueObjects;

namespace Timescaler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GetResultsContoller : ControllerBase
{
    private readonly IDataProcessingService _dataProcessingService;
    private readonly ILogger<GetResultsContoller> _logger;

    public GetResultsContoller(IDataProcessingService dataProcessingService, ILogger<GetResultsContoller> logger)
    {
        _dataProcessingService = dataProcessingService;
        _logger = logger;
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
}

using Microsoft.AspNetCore.Mvc;
using Timescaler.Application.Contracts;
using Timescaler.Application.Services.Interfaces;

namespace Timescaler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GetLastValuesController : ControllerBase
{
    private readonly IDataProcessingService _dataProcessingService;

    public GetLastValuesController(IDataProcessingService dataProcessingService, ILogger<GetLastValuesController> logger)
    {
        _dataProcessingService = dataProcessingService;
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

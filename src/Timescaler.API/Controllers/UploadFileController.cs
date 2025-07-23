using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Timescaler.Application.Services.Interfaces;

namespace Timescaler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadFileController : ControllerBase
{
    private readonly IDataProcessingService _dataProcessingService;
    private readonly ILogger<UploadFileController> _logger;

    public UploadFileController(IDataProcessingService dataProcessingService, ILogger<UploadFileController> logger)
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
}

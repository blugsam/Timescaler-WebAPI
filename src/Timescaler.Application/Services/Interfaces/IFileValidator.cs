using Timescaler.Application.Validation;

namespace Timescaler.Application.Services.Interfaces;

public interface IFileValidator
{
    Task<FileValidationResult> ValidateAsync(
        IAsyncEnumerable<(int LineNumber, string[] Fields)> parsedLines,
        CancellationToken ct = default);
}
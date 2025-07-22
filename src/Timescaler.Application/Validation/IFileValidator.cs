namespace Timescaler.Application.Validation;

public interface IFileValidator
{
    Task<FileValidationResult> ValidateAsync(
        IAsyncEnumerable<(int LineNumber, string[] Fields)> parsedLines,
        CancellationToken ct = default);
}

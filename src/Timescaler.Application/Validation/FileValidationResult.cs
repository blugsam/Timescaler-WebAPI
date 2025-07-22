using Timescaler.Application.Contracts;

namespace Timescaler.Application.Validation;

public record FileValidationResult
{
    public bool IsSuccess => !Errors.Any();
    public IReadOnlyList<ParsedValueRecord> ValidRecords { get; init; } = new List<ParsedValueRecord>();
    public IReadOnlyList<ValidationError> Errors { get; init; } = new List<ValidationError>();
}
namespace Timescaler.Application.Validation;

public record ValidationError(int LineNumber, string Message);
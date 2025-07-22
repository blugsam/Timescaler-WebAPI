namespace Timescaler.Application.Validation;

public record ParsedCsvRow(
    int LineNumber,
    DateTime Date,
    double ExecutionTime,
    decimal Value);
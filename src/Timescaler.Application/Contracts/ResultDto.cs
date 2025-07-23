namespace Timescaler.Application.Contracts;

public sealed record ResultDto(
    Guid Id,
    string FileName,
    DateTime FirstOperationDate,
    double TimeDelta,
    double AverageExecutionTime,
    decimal AverageValue,
    decimal MedianValue,
    decimal MaxValue,
    decimal MinValue);
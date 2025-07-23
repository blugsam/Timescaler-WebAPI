namespace Timescaler.Application.Contracts;

public sealed record RawValueDto(
    Guid Id,
    DateTime Date,
    double ExecutionTime,
    decimal Value);
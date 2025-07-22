namespace Timescaler.Domain.ValueObjects;

public sealed record RawDataPoint(DateTime Date, double ExecutionTime, decimal Value);

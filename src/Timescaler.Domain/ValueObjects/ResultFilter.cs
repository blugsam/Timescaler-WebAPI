namespace Timescaler.Domain.ValueObjects;

public sealed record ResultFilter(
    string? FileName = null,
    DateRange? FirstOperationDateRange = null,
    ValueRange? AverageValueRange = null,
    ExecutionTimeRange? AverageExecutionTimeRange = null);

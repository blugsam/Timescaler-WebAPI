namespace Timescaler.Application.Contracts;

public record ResultFilterDto(
    string? FileName,
    DateTime? StartDateFrom,
    DateTime? StartDateTo,
    decimal? AvgValueMin,
    decimal? AvgValueMax,
    double? AvgExecTimeMin,
    double? AvgExecTimeMax
);
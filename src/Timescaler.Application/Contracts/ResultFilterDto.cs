namespace Timescaler.Application.Contracts
{
    public record ResultFilterDto(
        string? FileName,
        DateTime? StartDateFrom,
        DateTime? StartDateTo,
        decimal? AvgValueMin,
        decimal? AvgValueMax,
        decimal? AvgExecTimeMin,
        decimal? AvgExecTimeMax
    );
}
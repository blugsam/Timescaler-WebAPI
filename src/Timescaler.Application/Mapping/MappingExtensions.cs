using Timescaler.Application.Contracts;
using Timescaler.Domain.Entities;

namespace Timescaler.Application.Mapping;

public static class MappingExtensions
{
    public static RawValueDto ToDto(this RawValue rawValue)
    {
        return new RawValueDto(
            rawValue.Id,
            rawValue.Date,
            rawValue.ExecutionTime,
            rawValue.Value);
    }

    public static ResultDto ToDto(this Result result)
    {
        return new ResultDto(
            result.Id,
            result.FileName,
            result.FirstOperationDate,
            result.TimeDelta,
            result.AverageExecutionTime,
            result.AverageValue,
            result.MedianValue,
            result.MaxValue,
            result.MinValue);
    }
}
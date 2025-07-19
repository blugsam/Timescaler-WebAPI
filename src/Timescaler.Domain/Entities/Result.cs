namespace Timescaler.Domain.Entities;

public class Result
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime FirstOperationDate { get; set; }
    public TimeSpan TimeDelta { get; set; }
    public double AverageExecutionTime { get; set; }
    public decimal AverageValue { get; set; }
    public decimal MedianValue { get; set; }
    public decimal MaxValue { get; set; }
    public decimal MinValue { get; set; }

    public ICollection<RawValue> RawValues { get; set; } = new List<RawValue>();
}

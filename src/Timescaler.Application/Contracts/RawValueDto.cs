namespace Timescaler.Application.Contracts;

public class RawValueDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public double ExecutionTime { get; set; }
    public decimal Value { get; set; }
    public Guid ResultId { get; set; }
}
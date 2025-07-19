namespace Timescaler.Domain.Entities;

public class ValueEntry
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public double ExecutionTime { get; set; }
    public decimal Value { get; set; }

    public Guid ResultEntryId { get; set; }
    public required ResultEntry ResultEntry { get; set; }
}
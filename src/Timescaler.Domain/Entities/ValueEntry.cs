namespace Timescaler.Domain.Entities
{
    public class ValueEntry
    {
        public Guid Id { get; set; }
        public string SourceFileName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public double ExecutionTime { get; set; }
        public double Value { get; set; }
    }
}

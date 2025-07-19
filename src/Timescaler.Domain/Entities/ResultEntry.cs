﻿namespace Timescaler.Domain.Entities
{
    public class ResultEntry
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime FirstOperationDate { get; set; }
        public double AverageExecutionTime { get; set; }
        public double AverageValue { get; set; }
        public double MedianValue { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
    }
}

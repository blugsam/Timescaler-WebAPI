namespace Timescaler.Application.Validation;

public record RawCsvRow(int LineNumber, string[] Fields);

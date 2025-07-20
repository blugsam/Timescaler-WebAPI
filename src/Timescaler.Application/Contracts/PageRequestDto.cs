namespace Timescaler.Application.Contracts;

public record PageRequestDto(int PageNumber, int PageSize)
{
    public int PageNumber { get; } = PageNumber < 1 ? 1 : PageNumber;
    public int PageSize { get; } = PageSize < 1 ? 10 : PageSize;
}
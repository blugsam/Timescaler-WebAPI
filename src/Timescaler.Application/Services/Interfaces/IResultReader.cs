using Timescaler.Application.Contracts;

namespace Timescaler.Application.Services.Interfaces;

public interface IResultReader
{
    Task<PagedListDto<ResultDto>> FindAsync(
    ResultFilterDto filter,
    PageRequestDto page,
    CancellationToken ct = default);
}
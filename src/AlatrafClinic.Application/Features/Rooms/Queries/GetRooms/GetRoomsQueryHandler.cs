using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Application.Features.Rooms.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Rooms.Queries.GetRooms;

public class GetRoomsQueryHandler
    : IRequestHandler<GetRoomsQuery, Result<PaginatedList<RoomDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRoomsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<RoomDto>>> Handle(
        GetRoomsQuery query,
        CancellationToken ct)
    {
        var specification = new RoomsFilter(query);

        var totalCount = await _unitOfWork.Rooms.CountAsync(specification, ct);

        var rooms = await _unitOfWork.Rooms
            .ListAsync(specification, specification.Page, specification.PageSize, ct);

        var items = rooms.ToDtos();

        return new PaginatedList<RoomDto>
        {
            Items      = items,
            PageNumber = specification.Page,
            PageSize   = specification.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)specification.PageSize)
        };
    }
}

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Organization.Rooms.Dtos;
using AlatrafClinic.Application.Features.Organization.Rooms.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Queries.GetRooms;

public sealed class GetRoomsQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetRoomsQuery, Result<List<RoomDto>>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public async Task<Result<List<RoomDto>>> Handle(GetRoomsQuery request, CancellationToken ct)
  {
    var rooms = await _unitOfWork.Rooms.GetAllRoomsFilteredAsync(
        request.SectionId,
        request.isActiveDoctor ,
        request.SearchTerm,
        ct
    );

    return rooms.ToDtos();
  }
}
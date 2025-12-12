using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetDoctorsBySectionRoom;

public sealed record GetDoctorsBySectionRoomQuery(int SectionId, int? RoomId = null) : IRequest<Result<List<GetDoctorDto>>>;

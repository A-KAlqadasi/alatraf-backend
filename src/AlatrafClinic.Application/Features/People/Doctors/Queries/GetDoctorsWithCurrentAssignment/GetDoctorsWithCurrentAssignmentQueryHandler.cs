using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.People.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Doctors.Queries.GetDoctorsWithCurrentAssignment;

public class GetDoctorsWithCurrentAssignmentQueryHandler
    : IRequestHandler<GetDoctorsWithCurrentAssignmentQuery, Result<PaginatedList<DoctorListItemDto>>>
{
  private readonly IUnitOfWork _uow;

  public GetDoctorsWithCurrentAssignmentQueryHandler(IUnitOfWork uow)
  {
    _uow = uow;
  }

  public async Task<Result<PaginatedList<DoctorListItemDto>>> Handle(GetDoctorsWithCurrentAssignmentQuery query, CancellationToken ct)
    {
        var spec = new DoctorsWithCurrentAssignmentFilter(query);

        var totalCount = await _uow.Doctors.CountAsync(spec, ct);

        var doctors = await _uow.Doctors
            .ListAsync(spec, spec.Page, spec.PageSize, ct);

        var items = doctors
            .Select(d => new DoctorListItemDto
            {
                DoctorId = d.Id,
                FullName = d.Person != null ? d.Person.FullName : string.Empty,
                Specialization = d.Specialization,
                DepartmentId = d.DepartmentId,
                DepartmentName = d.Department.Name,

                CurrentSectionId = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.SectionId)
                    .FirstOrDefault(),

                CurrentSectionName = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.Section.Name)
                    .FirstOrDefault(),

                CurrentRoomId = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.RoomId)
                    .FirstOrDefault(),

                CurrentRoomName = d.Assignments
                    .Where(a => a.IsActive && a.Room != null)
                    .Select(a => a.Room!.Name ?? string.Empty)
                    .FirstOrDefault(),

                AssignDate = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.AssignDate)
                    .FirstOrDefault(),

                IsActiveAssignment = d.Assignments.Any(a => a.IsActive)
            })
            .ToList();

        return new PaginatedList<DoctorListItemDto>
        {
            Items      = items,
            PageNumber = spec.Page,
            PageSize   = spec.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)spec.PageSize)
        };
    }
}
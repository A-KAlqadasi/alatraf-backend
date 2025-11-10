

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.People.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Doctors;

using MediatR;

using Microsoft.EntityFrameworkCore;

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
    // base query
    var doctorQuery = await _uow.Doctors.GetDoctorsQueryAsync(); // should return IQueryable<Doctor> with includes or projection base
    doctorQuery = ApplyFilters(doctorQuery, query);

    if (!string.IsNullOrWhiteSpace(query.Search))
      doctorQuery = ApplySearch(doctorQuery, query.Search!);

    doctorQuery = ApplySorting(doctorQuery, query.SortBy, query.SortDir);

    // paging guards
    var page = query.Page < 1 ? 1 : query.Page;
    var size = query.PageSize < 1 ? 10 : query.PageSize;

    var count = await doctorQuery.CountAsync(ct);

    var items = await doctorQuery
        .Skip((page - 1) * size)
        .Take(size)
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
          CurrentRoomNumber = d.Assignments
                .Where(a => a.IsActive && a.Room != null)
                .Select(a => a.Room.Number)
                .FirstOrDefault(),
          AssignDate = d.Assignments
                .Where(a => a.IsActive)
                .Select(a => a.AssignDate)
                .FirstOrDefault(),
          IsActiveAssignment = d.Assignments.Any(a => a.IsActive)
        })
        .ToListAsync(ct);

    return new PaginatedList<DoctorListItemDto>
    {
      Items = items,
      PageNumber = page,
      PageSize = size,
      TotalCount = count,
      TotalPages = (int)Math.Ceiling(count / (double)size)
    };
  }

  // ---------------- FILTERS ----------------
  private static IQueryable<Doctor> ApplyFilters(IQueryable<Doctor> query, GetDoctorsWithCurrentAssignmentQuery q)
  {
    if (q.DepartmentId.HasValue)
      query = query.Where(d => d.DepartmentId == q.DepartmentId.Value);

    if (!string.IsNullOrWhiteSpace(q.Specialization))
    {
      var spec = q.Specialization.Trim().ToLower();
      query = query.Where(d => d.Specialization != null &&
          EF.Functions.Like(d.Specialization.ToLower(), $"%{spec}%"));
    }

    if (q.SectionId.HasValue)
      query = query.Where(d => d.Assignments.Any(a => a.IsActive && a.SectionId == q.SectionId.Value));

    if (q.RoomId.HasValue)
      query = query.Where(d => d.Assignments.Any(a => a.IsActive && a.RoomId == q.RoomId.Value));

    if (q.HasActiveAssignment.HasValue)
    {
      if (q.HasActiveAssignment.Value)
        query = query.Where(d => d.Assignments.Any(a => a.IsActive));
      else
        query = query.Where(d => d.Assignments.All(a => !a.IsActive));
    }

    return query;
  }

  // ---------------- SEARCH ----------------
  private static IQueryable<Doctor> ApplySearch(IQueryable<Doctor> query, string term)
  {
    var pattern = $"%{term.Trim().ToLower()}%";
    return query.Where(d =>
        (d.Person != null && EF.Functions.Like(d.Person.FullName.ToLower(), pattern)) ||
        (d.Specialization != null && EF.Functions.Like(d.Specialization.ToLower(), pattern))
    );
  }

  // ---------------- SORTING ----------------
  private static IQueryable<Doctor> ApplySorting(IQueryable<Doctor> query, string sortBy, string sortDir)
  {
    var col = sortBy?.Trim().ToLowerInvariant() ?? "assigndate";
    var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

    return col switch
    {
      "name" => desc
          ? query.OrderByDescending(d => d.Person!.FullName)
          : query.OrderBy(d => d.Person!.FullName),

      "department" => desc
          ? query.OrderByDescending(d => d.Department.Name)
          : query.OrderBy(d => d.Department.Name),

      "specialization" => desc
          ? query.OrderByDescending(d => d.Specialization)
          : query.OrderBy(d => d.Specialization),

      "assigndate" => desc
          ? query.OrderByDescending(d => d.Assignments
              .Where(a => a.IsActive)
              .Select(a => a.AssignDate)
              .FirstOrDefault())
          : query.OrderBy(d => d.Assignments
              .Where(a => a.IsActive)
              .Select(a => a.AssignDate)
              .FirstOrDefault()),

      _ => query.OrderByDescending(d => d.Person!.FullName)
    };
  }
}

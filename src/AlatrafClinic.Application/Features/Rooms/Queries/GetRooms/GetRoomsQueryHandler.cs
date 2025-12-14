using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Application.Features.Rooms.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections.Rooms;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Rooms.Queries.GetRooms;

public class GetRoomsQueryHandler
    : IRequestHandler<GetRoomsQuery, Result<PaginatedList<RoomDto>>>
{
    private readonly IAppDbContext _context;

    public GetRoomsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<RoomDto>>> Handle(
        GetRoomsQuery query,
        CancellationToken ct)
    {
        IQueryable<Room> roomsQuery = _context.Rooms
            .Include(r => r.Section)
                .ThenInclude(s => s.Department)
            .AsNoTracking();

        roomsQuery = ApplyFilters(roomsQuery, query);
        roomsQuery = ApplySearch(roomsQuery, query);
        roomsQuery = ApplySorting(roomsQuery, query);

        var totalCount = await roomsQuery.CountAsync(ct);

        var page = query.Page;
        var pageSize = query.PageSize;
        var skip = (page - 1) * pageSize;

        var rooms = await roomsQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = rooms.ToDtos();

        return new PaginatedList<RoomDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static IQueryable<Room> ApplyFilters(
        IQueryable<Room> query,
        GetRoomsQuery q)
    {
        if (q.SectionId.HasValue && q.SectionId.Value > 0)
        {
            var sectionId = q.SectionId.Value;
            query = query.Where(r => r.SectionId == sectionId);
        }

        if (q.DepartmentId.HasValue && q.DepartmentId.Value > 0)
        {
            var depId = q.DepartmentId.Value;
            query = query.Where(r => r.Section.DepartmentId == depId);
        }

        return query;
    }

    private static IQueryable<Room> ApplySearch(
        IQueryable<Room> query,
        GetRoomsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(r =>
            EF.Functions.Like(r.Name.ToLower(), pattern) ||
            EF.Functions.Like(r.Section.Name.ToLower(), pattern) ||
            EF.Functions.Like(r.Section.Department.Name.ToLower(), pattern));
    }

    private static IQueryable<Room> ApplySorting(
        IQueryable<Room> query,
        GetRoomsQuery q)
    {
        var col = q.SortColumn?.Trim().ToLowerInvariant() ?? "name";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "id" => isDesc
                ? query.OrderByDescending(r => r.Id)
                : query.OrderBy(r => r.Id),

            "name" => isDesc
                ? query.OrderByDescending(r => r.Name)
                : query.OrderBy(r => r.Name),

            "section" => isDesc
                ? query.OrderByDescending(r => r.Section.Name)
                : query.OrderBy(r => r.Section.Name),

            "department" => isDesc
                ? query.OrderByDescending(r => r.Section.Department.Name)
                : query.OrderBy(r => r.Section.Department.Name),

            _ => isDesc
                ? query.OrderByDescending(r => r.Name)
                : query.OrderBy(r => r.Name),
        };
    }
}
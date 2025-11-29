using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Holidays.Dtos;
using AlatrafClinic.Application.Features.Holidays.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments.Holidays;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Holidays.Queries.GetHolidays;

public class GetHolidaysQueryHandler
    : IRequestHandler<GetHolidaysQuery, Result<PaginatedList<HolidayDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetHolidaysQueryHandler(IUnitOfWork uow)
    {
        _unitOfWork = uow;
    }

    public async Task<Result<PaginatedList<HolidayDto>>> Handle(
        GetHolidaysQuery query,
        CancellationToken ct)
    {
        var specification = new HolidaysFilter(query);

        // total count for pagination
        var totalCount = await _unitOfWork.Holidays.CountAsync(specification, ct);

        // data for this page
        var holidays = await _unitOfWork.Holidays
            .ListAsync(specification, specification.Page, specification.PageSize, ct);

        var items = holidays.ToDtos();

        return new PaginatedList<HolidayDto>
        {
            Items      = items,
            PageNumber = specification.Page,
            PageSize   = specification.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)specification.PageSize)
        };
    }
}

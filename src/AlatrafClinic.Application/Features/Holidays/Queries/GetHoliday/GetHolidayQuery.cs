using AlatrafClinic.Application.Features.Holidays.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Holidays.Queries.GetHoliday;

public sealed record GetHolidayQuery(
    int Id
) : IRequest<Result<HolidayDto>>;

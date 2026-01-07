using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Holidays.Commands.DeleteHoliday;

public sealed record DeleteHolidayCommand(int Id) : IRequest<Result<Deleted>>;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Holidays.Commands.DeleteHoliday;

public class DeleteHolidayCommandHandler : IRequestHandler<DeleteHolidayCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context;

    public DeleteHolidayCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Deleted>> Handle(DeleteHolidayCommand request, CancellationToken ct)
    {
        var holiday = await _context.Holidays.FirstOrDefaultAsync(h => h.Id == request.Id, ct);
        if (holiday is null)
        {
            return Error.NotFound("Holiday not found.");
        }

        _context.Holidays.Remove(holiday);
        await _context.SaveChangesAsync(ct);
        return Result.Deleted;
    }
}
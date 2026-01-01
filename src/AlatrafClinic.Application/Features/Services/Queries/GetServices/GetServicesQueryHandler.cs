using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Application.Features.Services.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Services.Queries.GetServices;

public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, Result<List<ServiceDto>>>
{
    private readonly IAppDbContext _context;

    public GetServicesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<ServiceDto>>> Handle(GetServicesQuery query, CancellationToken ct)
    {
        var services = await _context.Services.Include(s=> s.Department).ToListAsync(ct);
        
        return services.ToDtos();
    }
}
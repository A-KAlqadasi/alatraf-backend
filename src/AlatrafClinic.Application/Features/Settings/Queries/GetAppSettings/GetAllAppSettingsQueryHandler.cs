using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Settings.Dtos;
using AlatrafClinic.Application.Features.Settings.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Settings.Queries.GetAppSettings;

public sealed class GetAllAppSettingsQueryHandler(
    IAppDbContext context
)
    : IRequestHandler<GetAllAppSettingsQuery, Result<List<AppSettingDto>>>
{

    public async Task<Result<List<AppSettingDto>>> Handle(GetAllAppSettingsQuery request, CancellationToken ct)
    {
        var settings = await context.AppSettings.ToListAsync(ct);

        return settings.ToDtos();
    }
}
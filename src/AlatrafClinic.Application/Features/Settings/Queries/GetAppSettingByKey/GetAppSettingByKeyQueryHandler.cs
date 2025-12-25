using AlatrafClinic.Application.Common.Errors;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Settings.Dtos;
using AlatrafClinic.Application.Features.Settings.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Settings.Queries.GetAppSettingByKey;

public sealed class GetAppSettingByKeyQueryHandler(
    IAppDbContext _context
)
    : IRequestHandler<GetAppSettingByKeyQuery, Result<AppSettingDto>>
{

    public async Task<Result<AppSettingDto>> Handle(GetAppSettingByKeyQuery query, CancellationToken ct)
    {
        var setting = await _context.AppSettings.FirstOrDefaultAsync(a => a.Key == query.Key, ct);

        if (setting is null)
        return ApplicationErrors.AppSettingKeyNotFound;

        return setting.ToDto();
    }
}
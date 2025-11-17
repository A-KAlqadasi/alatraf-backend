
using AlatrafClinic.Application.Features.Settings.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Settings.Queries.GetAppSettingByKey;

public sealed record GetAppSettingByKeyQuery(string Key)
    : IRequest<Result<AppSettingDto>>;

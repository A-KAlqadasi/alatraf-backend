using AlatrafClinic.Application.Features.Settings.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Settings.Queries.GetAppSettings;

public sealed record GetAllAppSettingsQuery
    : IRequest<Result<List<AppSettingDto>>>;

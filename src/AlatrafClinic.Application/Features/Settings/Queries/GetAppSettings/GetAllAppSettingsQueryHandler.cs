using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Settings.Dtos;
using AlatrafClinic.Application.Features.Settings.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Settings.Queries.GetAppSettings;

public sealed class GetAllAppSettingsQueryHandler(

IUnitOfWork unitOfWork
)
    : IRequestHandler<GetAllAppSettingsQuery, Result<List<AppSettingDto>>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public async Task<Result<List<AppSettingDto>>> Handle(GetAllAppSettingsQuery request, CancellationToken cancellationToken)
  {
    var settings = await _unitOfWork.AppSettings.GetAllAsync(cancellationToken);

    return settings.ToDtos();
  }
}
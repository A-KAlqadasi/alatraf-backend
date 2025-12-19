using AlatrafClinic.Application.Common.Errors;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Settings.Dtos;
using AlatrafClinic.Application.Features.Settings.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Settings.Queries.GetAppSettingByKey;

public sealed class GetAppSettingByKeyQueryHandler(
    IUnitOfWork unitOfWork
)
    : IRequestHandler<GetAppSettingByKeyQuery, Result<AppSettingDto>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public async Task<Result<AppSettingDto>> Handle(GetAppSettingByKeyQuery request, CancellationToken cancellationToken)
  {
    var setting = await _unitOfWork.AppSettings.GetByKeyAsync(request.Key, cancellationToken);

    if (setting is null)
      return ApplicationErrors.AppSettingKeyNotFound;

    return setting.ToDto();
  }
}
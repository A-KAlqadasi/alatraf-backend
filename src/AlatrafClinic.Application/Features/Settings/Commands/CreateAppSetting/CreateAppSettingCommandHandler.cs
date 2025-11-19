using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Settings.Dtos;
using AlatrafClinic.Application.Features.Settings.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Settings;

using MediatR;

namespace AlatrafClinic.Application.Features.Settings.Commands;

public sealed class CreateAppSettingCommandHandler(
IUnitOfWork unitOfWork
)
    : IRequestHandler<CreateAppSettingCommand, Result<AppSettingDto>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public async Task<Result<AppSettingDto>> Handle(CreateAppSettingCommand request, CancellationToken cancellationToken)
  {
    var existing = await _unitOfWork.AppSettings.GetByKeyAsync(request.Key, cancellationToken);
    if (existing is not null)
      return AppSettingErrors.KeyAlreadyExists;

    var result = AppSetting.Create(
        request.Key,
        request.Value,
        request.Type,
        request.Description
    );

    if (result.IsError)
      return result.Errors;
    var newAppSetting = result.Value;
    await _unitOfWork.AppSettings.AddAsync(newAppSetting, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return newAppSetting.ToDto();
  }
}

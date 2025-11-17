using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Settings;

using MechanicShop.Application.Common.Errors;

using MediatR;

namespace AlatrafClinic.Application.Features.Settings.Commands.UpdateAppSetting;

public sealed class UpdateAppSettingCommandHandler(IUnitOfWork unitOfWor)
    : IRequestHandler<UpdateAppSettingCommand, Result<Updated>>
{
  private readonly IUnitOfWork _unitOfWor = unitOfWor;

  public async Task<Result<Updated>> Handle(UpdateAppSettingCommand request, CancellationToken cancellationToken)
  {
    var setting = await _unitOfWor.AppSettings.GetByKeyAsync(request.Key, cancellationToken);
    if (setting is null)
      return ApplicationErrors.AppSettingKeyNotFound;

    var result = setting.Update(request.Value, request.Description);
    if (result.IsError)
      return result.Errors;


    await _unitOfWor.AppSettings.UpdateAsync(setting, cancellationToken);

    return Result.Updated;
  }
}
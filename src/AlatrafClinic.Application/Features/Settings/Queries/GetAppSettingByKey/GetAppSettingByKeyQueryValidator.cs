using FluentValidation;

namespace AlatrafClinic.Application.Features.Settings.Queries.GetAppSettingByKey;

public sealed class GetAppSettingByKeyQueryValidator
    : AbstractValidator<GetAppSettingByKeyQuery>
{
    public GetAppSettingByKeyQueryValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .MaximumLength(200);
    }
}

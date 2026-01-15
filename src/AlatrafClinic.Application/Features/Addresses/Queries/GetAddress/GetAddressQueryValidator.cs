using FluentValidation;

namespace AlatrafClinic.Application.Features.Addresses.Queries.GetAddress;

public sealed class GetAddressQueryValidator : AbstractValidator<GetAddressQuery>
{
    public GetAddressQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
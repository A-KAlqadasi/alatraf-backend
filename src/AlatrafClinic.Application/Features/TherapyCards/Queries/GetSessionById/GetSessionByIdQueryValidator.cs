using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetSessionById;

public class GetSessionByIdQueryValidator : AbstractValidator<GetSessionByIdQuery>
{
    public GetSessionByIdQueryValidator()
    {
        RuleFor(x => x.SessionId).GreaterThan(0);
    }
}
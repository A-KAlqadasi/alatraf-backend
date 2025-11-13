using FluentValidation;

namespace AlatrafClinic.Application.Features.Tickets.Queries.GetTicketById;

public class GetTicketByIdQueryValidator : AbstractValidator<GetTicketByIdQuery>
{
    public GetTicketByIdQueryValidator()
    {
        RuleFor(x => x.ticketId)
            .GreaterThan(0).WithMessage("Ticket ID must be greater than zero.");
    }
}
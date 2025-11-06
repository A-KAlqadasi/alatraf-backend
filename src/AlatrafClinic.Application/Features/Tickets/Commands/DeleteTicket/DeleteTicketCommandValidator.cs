using FluentValidation;

namespace AlatrafClinic.Application.Features.Tickets.Commands.DeleteTicket;

public class DeleteTicketCommandValidator : AbstractValidator<DeleteTicketCommand>
{
    public DeleteTicketCommandValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0);
    }
}
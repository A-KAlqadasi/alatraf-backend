using FluentValidation;

namespace AlatrafClinic.Application.Features.Tickets.Commands.UpdateTicket;

public class UpdateTicketCommandValidator : AbstractValidator<UpdateTicketCommand>
{
    public UpdateTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("TicketId must be greater than 0.");

        RuleFor(x => x.ServiceId)
            .GreaterThan(0).WithMessage("ServiceId must be greater than 0.");

        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("PatientId must be greater than 0.");
    }
}
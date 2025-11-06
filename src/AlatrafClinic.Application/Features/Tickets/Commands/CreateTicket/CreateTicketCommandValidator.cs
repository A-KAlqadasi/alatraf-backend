using FluentValidation;

namespace AlatrafClinic.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0).WithMessage("ServiceId must be greater than 0.");

        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("PatientId must be greater than 0.");
    }
}
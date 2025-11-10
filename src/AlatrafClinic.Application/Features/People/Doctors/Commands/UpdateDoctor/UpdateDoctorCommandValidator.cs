using AlatrafClinic.Application.Features.People.Persons.Services;

using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.UpdateDoctor;

public sealed class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
{
    public UpdateDoctorCommandValidator()
    {
        // Primary keys
        RuleFor(x => x.DoctorId)
            .GreaterThan(0)
            .WithMessage("DoctorId must be greater than zero.");


        // Person fields
        RuleFor(x => x.Person)
             .NotNull()
             .SetValidator(new PersonInputValidator()); // 👈 reuse

        // Doctor fields
        RuleFor(x => x.Specialization)
            .NotEmpty().WithMessage("Specialization is required.")
            .MaximumLength(100).WithMessage("Specialization cannot exceed 100 characters.");


    }
}
using AlatrafClinic.Application.Features.People.Persons.Services;

using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.CreateDoctor;

public sealed class CreateDoctorCommandValidator : AbstractValidator<CreateDoctorCommand>
{
    public CreateDoctorCommandValidator()
    {
        RuleFor(x => x.Person)
                  .NotNull()
                  .SetValidator(new PersonInputValidator()); // 👈 reuse
                                                             // 🩺 Doctor-specific validation
        RuleFor(x => x.Specialization)
            .NotEmpty().WithMessage("Specialization is required.")
            .MaximumLength(100).WithMessage("Specialization cannot exceed 100 characters.");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("DepartmentId must be greater than zero.");
    }
}
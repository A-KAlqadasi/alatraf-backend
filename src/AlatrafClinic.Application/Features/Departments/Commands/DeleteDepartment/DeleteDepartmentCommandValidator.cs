using FluentValidation;

namespace AlatrafClinic.Application.Features.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentCommandValidator : AbstractValidator<DeleteDepartmentCommand>
{
    public DeleteDepartmentCommandValidator()
    {
        RuleFor(x=> x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("Department id is invalid");
    }
}
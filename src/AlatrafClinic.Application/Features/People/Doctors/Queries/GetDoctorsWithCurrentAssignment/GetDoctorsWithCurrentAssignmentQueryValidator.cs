using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Doctors.Queries.GetDoctorsWithCurrentAssignment;

public sealed class GetDoctorsWithCurrentAssignmentQueryValidator
    : AbstractValidator<GetDoctorsWithCurrentAssignmentQuery>
{
  public GetDoctorsWithCurrentAssignmentQueryValidator()
  {
    RuleFor(x => x.Page).GreaterThan(0);
    RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(200);

    RuleFor(x => x.SortDir)
        .Must(d => d is "asc" or "desc")
        .WithMessage("SortDir must be 'asc' or 'desc'.");

    RuleFor(x => x.SortBy)
        .Must(s => new[] { "Name", "Department", "Section", "Room", "AssignDate", "Specialization" }.Contains(s))
        .WithMessage("SortBy must be one of: Name, Department, Section, Room, AssignDate, Specialization.");
  }
}
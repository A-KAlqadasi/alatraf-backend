

using AlatrafClinic.Application.Features.People.Persons.Dtos;
using AlatrafClinic.Domain.Identity;

namespace AlatrafClinic.Application.Features.People.Employees.Dtos
{
    public class EmployeeDto
    {
        public Guid EmployeeId { get; set; }
        public int PersonId { get; set; }
        public PersonDto? Person { get; set; }
        public Role Role { get; set; }

    }
}
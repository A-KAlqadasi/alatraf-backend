using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Departments;
using AlatrafClinic.Domain.Organization.DoctorSectionRooms;
using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Domain.Organization.Doctors;

public class Doctor : AuditableEntity<int>
{
    public int PersonId { get; set; }
    public Person? Person { get; set; }
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public ICollection<DoctorSectionRoom> DoctorSectionRooms { get; set; } = new List<DoctorSectionRoom>();

    private Doctor()
    {
    }
    private Doctor(int personId, int? departmentId)
    {
        PersonId = personId;
        DepartmentId = departmentId;
    }
    public static Result<Doctor> Create(int personId, int? departmentId)
    {
        if (personId <= 0)
        {
            return DoctorErrors.PersonIdRequired;
        }

        if (departmentId is null || departmentId <= 0)
        {
            return DoctorErrors.DepartmentIdRequired;
        }

        return new Doctor(personId, departmentId);
    }
    
    public Result<Updated> Update(int personId, int? departmentId)
    {
        if (personId <= 0)
        {
            return DoctorErrors.PersonIdRequired;
        }

        if (departmentId is null || departmentId <= 0)
        {
            return DoctorErrors.DepartmentIdRequired;
        }

        PersonId = personId;
        DepartmentId = departmentId;

        return Result.Updated;
    }
}
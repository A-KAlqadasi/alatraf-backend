using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Organization.Doctors;


public static class DoctorErrors
{
    public static readonly Error PersonIdRequired =
        Error.Validation("Doctor.PersonIdRequired", "The person ID is required.");

    public static readonly Error DepartmentIdRequired =
        Error.Validation("Doctor.DepartmentIdRequired", "The department ID is required.");

    public static readonly Error SectionOutsideDepartment =
        Error.Validation("Doctor.SectionOutsideDepartment", "The section does not belong to the doctor’s department.");

    public static readonly Error RoomOutsideDepartment =
        Error.Validation("Doctor.RoomOutsideDepartment", "The room’s section does not belong to the doctor’s department.");

    public static readonly Error RoomWithoutSection =
        Error.Validation("Doctor.RoomWithoutSection", "The room must belong to a valid section.");

    public static readonly Error InvalidSpecializationForDepartment =
        Error.Validation("Doctor.InvalidSpecializationForDepartment", "The specialization does not exist in the department’s services.");
   public static readonly Error CannotChangeDepartmentWithActiveAssignments =
    Error.Validation("Doctor.CannotChangeDepartmentWithActiveAssignments",
        "The doctor cannot change department while having active assignments.");
    public static readonly Error SameDepartment =
    Error.Validation("Doctor.SameDepartment", "The new department must be different from the current department.");


}

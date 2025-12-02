using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.People.Doctors;


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
    public static readonly Error RoomOutsideSection = Error.Validation(
          code: "Doctor.RoomOutsideSection",
          description: "The specified room does not belong to the provided section.");

    public static readonly Error RoomOutsideActiveSection = Error.Validation(
        code: "Doctor.RoomOutsideActiveSection",
        description: "The new room must belong to the same section as the current active assignment.");

    public static readonly Error NoActiveAssignment = Error.Validation(
        code: "Doctor.NoActiveAssignment",
        description: "The doctor has no active assignment to update or modify.");

    public static readonly Error DoctorHasIndustrialPartsToday = Error.Validation(
        code: "Doctor.DoctorHasIndustrialPartsToday",
        description: "The doctor has industrial parts recorded for today and cannot be unassigned from the section.");
    
    public static readonly Error DoctorHasSessionsToday = Error.Validation(
        code: "Doctor.DoctorHasSessionsToday",
        description: "The doctor has sessions scheduled for today and cannot be unassigned from the section/room.");

}

using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.People.Doctors;


public static class DoctorErrors
{
    public static readonly Error DepartmentIdRequired =
        Error.Validation("Doctor.DepartmentIdRequired", "The department ID is required.");

    public static readonly Error SectionOutsideDepartment =
        Error.Validation("Doctor.SectionOutsideDepartment", "The section does not belong to the doctor’s department.");
    public static readonly Error RoomWithoutSection =
        Error.Validation("Doctor.RoomWithoutSection", "The room must belong to a valid section.");
    public static readonly Error CannotChangeDepartmentWithAssignments =
    Error.Conflict("Doctor.CannotChangeDepartmentWithAssignments",
         "The doctor cannot change department while already having assignments.");
    public static readonly Error RoomOutsideSection = Error.Validation(
          code: "Doctor.RoomOutsideSection",
          description: "The specified room does not belong to the provided section.");

    public static readonly Error NoActiveAssignment = Error.Validation(
        code: "Doctor.NoActiveAssignment",
        description: "The doctor has no active assignment to update or modify.");

    public static readonly Error DoctorHasIndustrialPartsToday = Error.Conflict(
        code: "Doctor.DoctorHasIndustrialPartsToday",
        description: "The doctor has industrial parts recorded for today and cannot be unassigned from the section.");
    
    public static readonly Error DoctorHasSessionsToday = Error.Conflict(
        code: "Doctor.DoctorHasSessionsToday",
        description: "The doctor has sessions scheduled for today and cannot be unassigned from the section/room.");
    public static readonly Error NotFound = Error.NotFound("Doctor.NotFound", "Doctor is not found");

}

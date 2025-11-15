
using AlatrafClinic.Domain.Common.Results;

namespace MechanicShop.Application.Common.Errors;

public static class ApplicationErrors
{
    // Every one update and add what he needs
    public static readonly Error PersonNotFound = Error.NotFound(
          code: "Person.NotFound",
          description: "Person not found.");
    public static readonly Error CannotDeleteReferencedPerson = Error.Conflict(
    "Person.CannotDeleteReferencedPerson",
         "Cannot delete this person because they are referenced by another entity (Doctor, Patient, or Employee).");
    public static Error PatientNotFound =>
Error.NotFound(
      "Patient.NotFound",
      "Patient does not exist.");


    public static Error PersonAlreadyAssigned(int personId) => Error.Conflict(
        code: "Person.AlreadyAssigned",
        description: $"Person with Id {personId} is already registered as Doctor, Employee, or Patient.");


    public static Error PatientAlreadyExists(int personId) => Error.Conflict(
        code: "Patient.AlreadyExists",
        description: $"A patient already exists for person Id {personId}.");

    public static Error DoctorAlreadyExists(string nationalNo) => Error.Conflict(
        code: "Doctor.AlreadyExists",
        description: $"A doctor already exists for NationalNo  {nationalNo}  .");

    public static Error EmployeeAlreadyExists(int personId) => Error.Conflict(
        code: "Employee.AlreadyExists",
        description: $"An employee already exists for person Id {personId}.");
    public static readonly Error EmployeeNotFound = Error.NotFound(
        "Employee.NotFound",
        "Employee does not exist."
    );


    public static readonly Error DepartmentNotFound =
        Error.NotFound("Doctor.DepartmentNotFound", "The specified department does not exist.");
    public static readonly Error DoctorNotFound =
      Error.NotFound("Doctor.DoctorNotFound", "The specified doctor does not exist.");
    public static readonly Error SectionNotFound = Error.Validation(
           code: "Section.NotFound",
           description: "The section was not found in this department.");

    public static readonly Error RoomNotFound = Error.Validation(
   code: "Room.NotFound",
   description: "The Room was not found in this Section.");
    // Department related Errors
    public static Error DepartmentAlreadyExists(string name) =>
Error.Conflict("Department.AlreadyExists", $"Department with name '{name}' already exists.");
    // Sections related Errors

    public static Error SectionAlreadyExists(string name) =>
    Error.Conflict("Section.AlreadyExists", $"Section with name '{name}' already exists in this department.");

public static readonly Error RoomHasActiveDoctorAssignment =
    Error.Conflict("Room.HasActiveDoctorAssignment", "Room cannot be deleted because it has active doctor assignments.");

  public static Error HolidayAlreadyExists(DateTime date) =>
    Error.Conflict("Holiday.AlreadyExists", $"Holiday with date '{date}' already exists.");

  public static readonly Error HolidayNotFound = Error.Validation(
   code: "Holiday.NotFound",
   description: "The Holiday not found .");
}
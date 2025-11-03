
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

    public static Error DoctorAlreadyExists(int personId) => Error.Conflict(
        code: "Doctor.AlreadyExists",
        description: $"A doctor already exists for person Id {personId}.");

    public static Error EmployeeAlreadyExists(int personId) => Error.Conflict(
        code: "Employee.AlreadyExists",
        description: $"An employee already exists for person Id {personId}.");
    public static readonly Error EmployeeNotFound = Error.NotFound(
        "Employee.NotFound",
        "Employee does not exist."
    );
}
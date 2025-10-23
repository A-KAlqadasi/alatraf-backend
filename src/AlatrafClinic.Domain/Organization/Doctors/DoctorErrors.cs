using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Organization.Doctors;

public static class DoctorErrors
{
    public static readonly Error PersonIdRequired =
        Error.Validation("Doctor.PersonIdRequired", "Doctor person Id is required.");
        
    public static readonly Error DepartmentIdRequired =
        Error.Validation("Doctor.DepartmentIdRequired", "Doctor department Id is required.");
}
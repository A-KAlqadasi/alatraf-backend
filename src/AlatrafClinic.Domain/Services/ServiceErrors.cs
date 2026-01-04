using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Services;

public static class ServiceErrors
{
    public static readonly Error NameIsRequired = Error.Validation("Service.NameIsRequired", "Service name is required.");
    public static readonly Error DepartmentIdIsRequired = Error.Validation("Service.DepartmentIdIsRequired", "Department ID is required.");
    public static readonly Error ServiceNotFound = Error.NotFound("Service.NotFound", "The specified service was not found.");
    public static readonly Error InvalidServicePrice = Error.Validation("Service.InvalidServicePrice", "The service price must be greater than zero.");
    public static readonly Error ServiceIdInvalid = Error.Validation("Service.InvalidServiceId", "Service id is invalid");
}
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.People;

public static class PersonErrors
{
    public static Error NameRequired =>
        Error.Validation("Person.NameRequired", "Person name is required");

    public static Error PhoneNumberRequired =>
        Error.Validation("Person.PhoneNumberRequired", "Phone number is required");

    public static Error PhoneExists =>
        Error.Conflict("Person.PhoneExists", "A person with this phone already exists.");
    
    public static Error NationalNoExists =>
        Error.Conflict("Person.NationalNoExists", "A person with this national number already exists.");
    
    public static readonly Error InvalidPhoneNumber =
        Error.Conflict("Person.InvalidPhoneNumber", "Phone number must be 9 digits and may start with '77', '78', '73', or '71'.");
    public static readonly Error BirthdateRequired =
        Error.Validation("Person.BirthdateRequired", "Birthdate is required.");
    public static readonly Error InvalidBirthdate =
        Error.Validation("Person.InvalidBirthdate", "Birthdate cannot be in the future.");

    public static readonly Error CannotDeletePersonWithWorkOrders =
        Error.Conflict("Person.CannotDelete", "Person cannot be deleted due to existing relations with other services.");
    public static readonly Error AddressRequired =
        Error.Validation("Person.AddressRequired", "Address is required.");
}
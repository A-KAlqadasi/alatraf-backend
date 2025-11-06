using System.Text.RegularExpressions;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.People;

public sealed class Person : AuditableEntity<int>
{
    public string FullName { get; private set; }

    public DateTime Birthdate { get; private set; }

    public string Phone { get; private set; }
    
    public string? NationalNo { get; private set; }

    public string Address { get; private set; }
    private Person() { }

    private Person(string fullname, DateTime birthdate, string phone, string? nationalNo, string address)
    {
        FullName = fullname;
        Birthdate = birthdate;
        Phone = phone;
        NationalNo = nationalNo;
        Address = address;
    }

    public static Result<Person> Create(string fullname, DateTime birthdate, string phone, string? nationalNo, string address)
    {
         
        if (string.IsNullOrWhiteSpace(fullname))
        {
            return PersonErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^(77|78|73|71)\d{7}$"))
        {
            return PersonErrors.InvalidPhoneNumber;
        }

        if (birthdate > DateTime.UtcNow)
        {
            return PersonErrors.InvalidBirthdate;
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            return PersonErrors.AddressRequired;
        }

        return new Person(fullname, birthdate, phone, nationalNo, address);
    }

    public Result<Updated> Update(string fullname, DateTime birthdate, string phone, string? nationalNo, string address)
    {
        
        
        if (string.IsNullOrWhiteSpace(fullname))
        {
            return PersonErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^(77|78|73|71)\d{7}$"))
        {
            return PersonErrors.InvalidPhoneNumber;
        }

        if (birthdate > DateTime.UtcNow)
        {
            return PersonErrors.InvalidBirthdate;
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            return PersonErrors.AddressRequired;
        }
        
        FullName = fullname;
        Birthdate = birthdate;
        Phone = phone;
        NationalNo = nationalNo;
        Address = address;

        return Result.Updated;
    }
}
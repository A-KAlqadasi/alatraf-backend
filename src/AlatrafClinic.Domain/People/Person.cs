using System.Text.RegularExpressions;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.People.Doctors;

namespace AlatrafClinic.Domain.People;

public sealed class Person : AuditableEntity<int>
{
    public string FullName { get; private set; } = null!;
    public DateOnly Birthdate { get; private set; }
    public int Age { get; private set; }
    public string Phone { get; private set; } = null!;
    public string? NationalNo { get; private set; }
    public bool Gender { get; private set; } // Added: true = Male, false = Female
    public string Address { get; private set; } = null!;
    public string? AutoRegistrationNumber { get; set; }

    public Patient? Patient { get; private set; }
    public Doctor? Doctor { get; private set; }

    private Person() { }

    private Person(string fullname, DateOnly birthdate, string phone, string? nationalNo, string address, bool gender, int age)
    {
        FullName = fullname;
        Birthdate = birthdate;
        Phone = phone;
        NationalNo = nationalNo;
        Address = address;
        Gender = gender;
        Age = age;
    }

    public static Result<Person> Create(string fullname, DateOnly birthdate, string phone, string? nationalNo, string address, bool gender)
    {
        if (string.IsNullOrWhiteSpace(fullname))
            return PersonErrors.NameRequired;

        if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^(77|78|73|71)\d{7}$"))
            return PersonErrors.InvalidPhoneNumber;

        if (birthdate > AlatrafClinicConstants.TodayDate)
            return PersonErrors.InvalidBirthdate;

        if (string.IsNullOrWhiteSpace(address))
            return PersonErrors.AddressRequired;
        
        int age = CalculateAge(birthdate, AlatrafClinicConstants.TodayDate);

        return new Person(fullname, birthdate, phone, nationalNo, address, gender, age);
    }

    private static int CalculateAge(DateOnly dateOfBirth, DateOnly now)
    {
        var age = now.Year - dateOfBirth.Year;

        if (dateOfBirth > now.AddYears(-age))
        {
            age -= 1;
        }

        return age;
    }

    public Result<Updated> Update(string fullname, DateOnly birthdate, string phone, string? nationalNo, string address, bool gender)
    {
        if (string.IsNullOrWhiteSpace(fullname))
            return PersonErrors.NameRequired;

        if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^(77|78|73|71)\d{7}$"))
            return PersonErrors.InvalidPhoneNumber;

        if (birthdate > AlatrafClinicConstants.TodayDate)
            return PersonErrors.InvalidBirthdate;

        if (string.IsNullOrWhiteSpace(address))
            return PersonErrors.AddressRequired;

        FullName = fullname;
        Birthdate = birthdate;
        Phone = phone;
        NationalNo = nationalNo;
        Address = address;
        Gender = gender;
        Age = CalculateAge(birthdate, AlatrafClinicConstants.TodayDate);

        return Result.Updated;
    }
    public Result<Updated> AssignPatient(Patient patient)
    {
        if(patient is null)
        {
            return PersonErrors.PatientIsRequired;
        }
        Patient = patient;

        return Result.Updated;
    }
    public Result<Updated> AssignDoctor(Doctor doctor)
    {
        if(doctor is null)
        {
            return PersonErrors.DoctorIsRequired;
        }
        Doctor = doctor;

        return Result.Updated;
    }
}

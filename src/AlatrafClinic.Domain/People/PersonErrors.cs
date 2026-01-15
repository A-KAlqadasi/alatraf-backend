using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.People;

public static class PersonErrors
{
    public static Error NameRequired =>
        Error.Validation("Person.NameRequired", "اسم الشخص مطلوب");

    public static Error PhoneNumberRequired =>
        Error.Validation("Person.PhoneNumberRequired", "رقم الهاتف مطلوب");

    public static Error PhoneExists =>
        Error.Conflict("Person.PhoneExists", "هذا الرقم مسجل مسبقاً");
    
    public static Error NationalNoExists =>
        Error.Conflict("Person.NationalNoExists", "رقم الهوية الوطنية مسجل مسبقاً");
    
    public static readonly Error InvalidPhoneNumber =
        Error.Conflict("Person.InvalidPhoneNumber", "يجب أن يكون رقم الهاتف مكونًا من 9 أرقام وقد يبدأ بـ '77' أو '78' أو '73' أو '71'");
    public static readonly Error InvalidBirthdate =
        Error.Validation("Person.InvalidBirthdate", "لا يمكن أن يكون تاريخ الميلاد في المستقبل");
    public static readonly Error AddressRequired =
        Error.Validation("Person.AddressRequired", "العنوان مطلوب.");
    public static readonly Error NameIsExist = Error.Conflict("Person.NameIsExist", "الاسم موجود بالفعل");

    public static readonly Error NotFound = Error.NotFound("Person.NotFound", "الشخص غير موجود");

    public static readonly Error PatientIsRequired = Error.Validation("Person.PatientIsRequired", "المريض مطلوب ليتم تعيينه للشخص");

    public static readonly Error DoctorIsRequired = Error .Validation("Person.DoctorIsRequired", "الطبيب مطلوب ليتم تعيينه للشخص");
    public static readonly Error PersonNotFound = Error.NotFound("Person.PersonNotFound", "الشخص غير موجود");
    public static readonly Error AddressNotFound = Error.NotFound("Person.AddressNotFound", "العنوان غير موجود");
    public static readonly Error AddressIdIsAlreadyInUse = Error.Conflict("Person.AddressIdIsAlreadyInUse", "معرف العنوان مستخدم مسبقاً");
    public static readonly Error AddressNameIsAlreadyExist = Error.Conflict("Person.AddressNameIsAlreadyInUse", "اسم العنوان موجود مسبقاً");
    
}
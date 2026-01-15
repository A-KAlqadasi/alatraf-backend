using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments;

public static class PaymentErrors
{
    public static readonly Error InvalidTotal =
        Error.Validation("Payment.InvalidTotal", "المبلغ الإجمالي يجب أن يكون موجبا");

    public static readonly Error InvalidPaid =
        Error.Validation("Payment.InvalidPaid", "المبلغ المدفوع يجب أن يكون موجبا");

    public static readonly Error InvalidDiscount =
        Error.Validation("Payment.InvalidDiscount", "الخصم لا يمكن أن يكون سالبا");

    public static readonly Error OverPayment =
        Error.Conflict("Payment.OverPayment", "المبلغ المدفوع والخصم يتجاوزان المبلغ الإجمالي المطلوب");
    public static readonly Error InvalidDiagnosisId =
        Error.Validation("Payment.InvalidDiagnosisId", "معرف التشخيص غير صالح");
    public static readonly Error InvalidPaymentReference = Error.Validation("Payment.InvalidPaymentReference", "مرجع الدفع غير صالح");
    public static readonly Error PaymentAlreadyCompleted = Error.Conflict("Payment.PaymentAlreadyCompleted", "تم إكمال الدفع بالفعل");
    public static readonly Error InvalidPatientPayment = Error.Validation("Payment.InvalidPatientPayment", "مدفوعات المرضى يجب أن تكون من نوع حساب المرضى");
    public static readonly Error InvalidDisabledPayment = Error.Validation("Payment.InvalidDisabledPayment", "مدفوعات المعاقين يجب أن تكون من نوع حساب المعاقين");
    public static readonly Error InvalidWoundedPayment = Error.Validation("Payment.InvalidWoundedPayment", "مدفوعات الجرحى يجب أن تكون من نوع حساب الجرحى");
    public static readonly Error PaymentNotFound = Error.NotFound("Payment.PaymentNotFound", "الدفع غير موجود");
    public static readonly Error InvalidAccountKind = Error.Conflict("Payment.InvalidAccountKind", "نوع الحساب غير صالح");
    public static readonly Error InvalidTicketId = Error.Validation("Payment.InvalidTicketId", "معرف التذكرة غير صالح");

    public static readonly Error UnderPayment =
        Error.Conflict("Payment.UnderPayment", "المبلغ المدفوع والخصم لا يغطيان المبلغ الإجمالي المطلوب");
}

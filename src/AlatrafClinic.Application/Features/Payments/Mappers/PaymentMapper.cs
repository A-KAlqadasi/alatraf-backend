using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Mappers;

public static class PaymentMapper
{
    
    public static PaymentDto ToDto(this Payment p)
    {
        ArgumentNullException.ThrowIfNull(p);
        return new PaymentDto
        {
            PaymentId = p.Id,
            TicketId = p.TicketId,
            PaymentReference = p.PaymentReference,
            Diagnosis = p.Diagnosis != null ? p.Diagnosis.ToDto() : new DiagnosisDto(),
            AccountKind = p.AccountKind,
            IsCompleted = p.IsCompleted,
            TotalAmount = p.TotalAmount,
            PaidAmount = p.PaidAmount,
            Discount = p.Discount,
            PatientPayment = p.PatientPayment != null
                    ? new PatientPaymentDto
                    {
                        VoucherNumber = p.PatientPayment.VoucherNumber,
                        Notes = p.PatientPayment.Notes
                    }
                    : null,
            DisabledPayment = p.DisabledPayment != null
                    ? new DisabledPaymentDto
                    {
                        DisabledCardId = p.DisabledPayment.DisabledCardId,
                        Notes = p.DisabledPayment.Notes
                    }
                    : null,
            WoundedPayment = p.WoundedPayment != null
                    ? new WoundedPaymentDto
                    {
                        WoundedCardId = p.WoundedPayment.WoundedCardId,
                        ReportNumber = p.WoundedPayment.ReportNumber,
                        Notes = p.WoundedPayment.Notes
                    }
                    : null
        };
    }
    public static List<PaymentDto> ToDtos(this IEnumerable<Payment> payments)
    {
        return payments.Select(p => p.ToDto()).ToList();
    }

    public static PaymentWaitingListDto ToPaymentWaitingListDto(this Payment entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var cardId = entity.PaymentReference == PaymentReference.Repair ? entity.Diagnosis?.RepairCard?.Id : ((entity.PaymentReference == PaymentReference.Sales) ? entity.Diagnosis?.Sale?.Id : entity.Diagnosis?.TherapyCard?.Id );
        var birthdate = entity.Diagnosis?.Patient.Person.Birthdate ?? DateOnly.MinValue;
        return new PaymentWaitingListDto
        {
            PaymentId = entity.Id,
            CardId = cardId ?? 0,
            PatientName = entity.Diagnosis?.Patient.Person.FullName ?? string.Empty,
            Age = UtilityService.CalculateAge(birthdate, AlatrafClinicConstants.TodayDate),
            Gender = UtilityService.GenderToArabicString(entity.Diagnosis?.Patient.Person.Gender ?? true),
            Phone = entity.Diagnosis?.Patient.Person.Phone,
            PaymentReference = entity.PaymentReference
            
        };
    }
    public static List<PaymentWaitingListDto> ToPaymentWaitingListDtos(this IEnumerable<Payment> entities)
    {
        return [..entities.Select(p=> p.ToPaymentWaitingListDto())];
    }


    public static string ToArabicPaymentReference(this PaymentReference paymentReference)
    {
        return paymentReference switch
        {
            PaymentReference.Repair => "إصلاحات فنية",
            PaymentReference.TherapyCardRenew => "تجديد كرت",
            PaymentReference.TherapyCardNew => "علاج طبيعي",
            PaymentReference.Sales => "مبيعات",
            PaymentReference.TherapyCardDamagedReplacement => "بدل فاقد",
            _ => "غير معروف"
        };

    }

     public static string ToArabicAccountKind(this AccountKind? accountKind)
    {
        return accountKind switch
        {
            AccountKind.Free => "حساب المجان",
            AccountKind.Patient => "حساب المرضى",
            AccountKind.Disabled => "حساب المعاقين",
            AccountKind.Wounded => "حساب الجرحى",
            _ => "غير معروف"
        };

    }

    public static TherapyPaymentDto ToTherapyPaymentDto(this Payment payment)
    {
        ArgumentNullException.ThrowIfNull(payment);
        var birthdate = payment.Diagnosis.Patient.Person.Birthdate ;
        bool gender = payment.Diagnosis.Patient.Person.Gender;
        return new TherapyPaymentDto
        {
            PaymentId = payment.Id,

            PatientName = payment.Diagnosis.Patient.Person.FullName,

            Age = UtilityService.CalculateAge(birthdate, AlatrafClinicConstants.TodayDate),

            Gender = UtilityService.GenderToArabicString(gender),
            PatientId = payment.Diagnosis.Patient.Id,
            DiagnosisPrograms = payment.Diagnosis?.DiagnosisPrograms?.ToDtos() ?? new(),
            IsCompleted = payment.IsCompleted,
            TotalAmount = payment.TotalAmount,
            PaidAmount = payment.PaidAmount,
            Discount  = payment.Discount,
            AccountKind = payment.AccountKind.ToArabicAccountKind(),
            PaymentDate = payment.PaymentDate
        };
    }

    public static RepairPaymentDto ToRepairPaymentDto(this Payment payment)
    {
        ArgumentNullException.ThrowIfNull(payment);
        var birthdate = payment.Diagnosis.Patient.Person.Birthdate ;
        bool gender = payment.Diagnosis.Patient.Person.Gender;
        return new RepairPaymentDto
        {
            PaymentId = payment.Id,

            PatientName = payment.Diagnosis.Patient.Person.FullName,

            Age = UtilityService.CalculateAge(birthdate, AlatrafClinicConstants.TodayDate),

            Gender = UtilityService.GenderToArabicString(gender),
            PatientId = payment.Diagnosis.Patient.Id,
            DiagnosisIndustrialParts = payment.Diagnosis?.DiagnosisIndustrialParts?.ToDtos() ?? new(),
            IsCompleted = payment.IsCompleted,
            TotalAmount = payment.TotalAmount,
            PaidAmount = payment.PaidAmount,
            Discount  = payment.Discount,
            AccountKind = payment.AccountKind.ToArabicAccountKind(),
            PaymentDate = payment.PaymentDate
        };
    }

    
}
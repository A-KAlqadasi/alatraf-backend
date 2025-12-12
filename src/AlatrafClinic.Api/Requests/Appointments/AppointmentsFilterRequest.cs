using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.Services.Enums;


namespace AlatrafClinic.Api.Requests.Appointments;

public sealed class AppointmentsFilterRequest
{
    [MaxLength(200)]
    public string? SearchTerm { get; init; }

    [EnumDataType(typeof(AppointmentStatus))]
    public AppointmentStatus? Status { get; init; }

    [EnumDataType(typeof(PatientType))]
    public PatientType? PatientType { get; init; }

    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }

    [Required]
    [MaxLength(50)]
    public string SortColumn { get; init; } = "AttendDate";

    [Required]
    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'.")]
    public string SortDirection { get; init; } = "asc";
}


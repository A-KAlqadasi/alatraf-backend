namespace AlatrafClinic.Domain.Common.Constants;

public static class AlatrafClinicConstants
{
    public const string DefaultColor = "#003366";
    public const string SystemUser = "System";
    public const string AllowedDaysKey = "AllowedAppointmentDays";
    public const string AppointmentDailyCapacityKey = "AppointmentDailyCapacity";
    public const string WoundedReportMinTotalKey = "WoundedReportMinTotal";
    public const int DefaultAppointmentDailyCapacity = 10;

    public static readonly DateOnly TodayDate = DateOnly.FromDateTime(DateTime.Now);
   
    public static class DatabaseTables
    {
        public const string Appointments = "Appointments";
        public const string AppSettings = "AppSettings";
        public const string AspNetRoleClaims = "AspNetRoleClaims";
        public const string AspNetRoles = "AspNetRoles";
        public const string AspNetUserClaims = "AspNetUserClaims";
        public const string AspNetUserLogins = "AspNetUserLogins";
        public const string AspNetUserRoles = "AspNetUserRoles";
        public const string AspNetUsers = "AspNetUsers";
        public const string AspNetUserTokens = "AspNetUserTokens";
        public const string DeliveryTimes = "DeliveryTimes";
        public const string Departments = "Departments";
        public const string Diagnoses = "Diagnoses";
        public const string DiagnosisIndustrialParts = "DiagnosisIndustrialParts";
        public const string DiagnosisInjuryReason = "DiagnosisInjuryReason";
        public const string DiagnosisInjurySide = "DiagnosisInjurySide";
        public const string DiagnosisInjuryType = "DiagnosisInjuryType";
        public const string DiagnosisPrograms = "DiagnosisPrograms";
        public const string DisabledCards = "DisabledCards";
        public const string DisabledPayments = "DisabledPayments";
        public const string Doctors = "Doctors";
        public const string DoctorSectionRooms = "DoctorSectionRooms";
        public const string ExchangeOrderItems = "ExchangeOrderItems";
        public const string ExchangeOrders = "ExchangeOrders";
        public const string ExitCards = "ExitCards";
        public const string Holidays = "Holidays";
        public const string IndustrialParts = "IndustrialParts";
        public const string IndustrialPartUnits = "IndustrialPartUnits";
        public const string InjuryReasons = "InjuryReasons";
        public const string InjurySides = "InjurySides";
        public const string InjuryTypes = "InjuryTypes";
        public const string Items = "Items";
        public const string ItemUnits = "ItemUnits";
        public const string MedicalPrograms = "MedicalPrograms";
        public const string OrderItems = "OrderItems";
        public const string Orders = "Orders";
        public const string PatientPayments = "PatientPayments";
        public const string Patients = "Patients";
        public const string Payments = "Payments";
        public const string People = "People";
        public const string Permissions = "Permissions";
        public const string PurchaseInvoices = "PurchaseInvoices";
        public const string PurchaseItems = "PurchaseItems";
        public const string RefreshTokens = "RefreshTokens";
        public const string RepairCards = "RepairCards";
        public const string ReportDomains = "ReportDomains";
        public const string ReportFields = "ReportFields";
        public const string ReportJoins = "ReportJoins";
        public const string RolePermissions = "RolePermissions";
        public const string Rooms = "Rooms";
        public const string SaleItems = "SaleItems";
        public const string Sales = "Sales";
        public const string Sections = "Sections";
        public const string Services = "Services";
        public const string SessionPrograms = "SessionPrograms";
        public const string Sessions = "Sessions";
        public const string StoreItemUnits = "StoreItemUnits";
        public const string Stores = "Stores";
        public const string Suppliers = "Suppliers";
        public const string TherapyCards = "TherapyCards";
        public const string TherapyCardTypePrices = "TherapyCardTypePrices";
        public const string Tickets = "Tickets";
        public const string Units = "Units";
        public const string UserPermissionOverrides = "UserPermissionOverrides";
        public const string WoundedCards = "WoundedCards";
        public const string WoundedPayments = "WoundedPayments";
    }

    public static class PatientReport
    {   
        public const string PatientId = "patient_id";
        public const string TicketId = "patient_ticketId";
        public const string ServiceType = "patient_service_type";
    }
}
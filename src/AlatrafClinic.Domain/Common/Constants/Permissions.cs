namespace AlatrafClinic.Domain.Common.Constants;

public static class Permission
{
    public static class Person
    {
        public const int CreateId = 1;
        public const int ReadId = 2;
        public const int UpdateId = 3;
        public const int DeleteId = 4;

        public const string Create = "person:create";
        public const string Read = "person:read";
        public const string Update = "person:update";
        public const string Delete = "person:delete";
    }

    public static class Service
    {
        public const int CreateId = 5;
        public const int ReadId = 6;
        public const int UpdateId = 7;
        public const int DeleteId = 8;

        public const string Create = "service:create";
        public const string Read = "service:read";
        public const string Update = "service:update";
        public const string Delete = "service:delete";
    }

    public static class Ticket
    {
        public const int CreateId = 9;
        public const int ReadId = 10;
        public const int UpdateId = 11;
        public const int DeleteId = 12;
        public const int PrintId = 13;

        public const string Create = "ticket:create";
        public const string Read = "ticket:read";
        public const string Update = "ticket:update";
        public const string Delete = "ticket:delete";
        public const string Print = "ticket:print";
    }

    public static class Appointment
    {
        public const int CreateId = 14;
        public const int ReScheduleId = 15;
        public const int ReadId = 16;
        public const int UpdateId = 17;
        public const int DeleteId = 18;
        public const int ChangeStatusId = 19;
        public const int PrintId = 20;

        public const string Create = "appointment:create";
        public const string ReSchedule = "appointment:reschedule";
        public const string Read = "appointment:read";
        public const string Update = "appointment:update";
        public const string Delete = "appointment:delete";
        public const string ChangeStatus = "appointment:change_status";
        public const string Print = "appointment:print";
    }

    public static class Holiday
    {
        public const int CreateId = 21;
        public const int ReadId = 22;
        public const int UpdateId = 23;
        public const int DeleteId = 24;

        public const string Create = "holiday:create";
        public const string Read = "holiday:read";
        public const string Update = "holiday:update";
        public const string Delete = "holiday:delete";
    }

    public static class TherapyCard
    {
        public const int CreateId = 25;
        public const int ReadId = 26;
        public const int UpdateId = 27;
        public const int DeleteId = 28;
        public const int RenewId = 29;
        public const int DamageReplacementId = 30;
        public const int CreateSessionId = 31;
        public const int ReadSessionId = 32;
        public const int PrintTherapyCardId = 33;
        public const int PrintSessionId = 34;

        public const string Create = "therapyCard:create";
        public const string Read = "therapyCard:read";
        public const string Update = "therapyCard:update";
        public const string Delete = "therapyCard:delete";
        public const string Renew = "therapyCard:renew";
        public const string DamageReplacement = "therapyCard:damage_replacement";
        public const string CreateSession = "therapyCard:create_session";
        public const string ReadSession = "therapyCard:read_session";
        public const string PrintTherapyCard = "therapyCard:print_therapy_card";
        public const string PrintSession = "therapyCard:print_session";
    }

    public static class RepairCard
    {
        public const int CreateId = 35;
        public const int ReadId = 36;
        public const int UpdateId = 37;
        public const int DeleteId = 38;
        public const int ChangeStatusId = 39;
        public const int AssignToTechnicianId = 40;
        public const int CreateDeliveryTimeId = 41;
        public const int PrintRepairCardId = 42;
        public const int PrintDeliveryTimeId = 43;

        public const string Create = "repairCard:create";
        public const string Read = "repairCard:read";
        public const string Update = "repairCard:update";
        public const string Delete = "repairCard:delete";
        public const string ChangeStatus = "repairCard:change_status";
        public const string AssignToTechnician = "repairCard:assign_to_technician";
        public const string CreateDeliveryTime = "repairCard:create_delivery_time";
        public const string PrintRepairCard = "repairCard:print_repair_card";
        public const string PrintDeliveryTime = "repairCard:print_delivery_time";
    }

    public static class IndustrialPart
    {
        public const int CreateId = 44;
        public const int ReadId = 45;
        public const int UpdateId = 46;
        public const int DeleteId = 47;

        public const string Create = "industrialPart:create";
        public const string Read = "industrialPart:read";
        public const string Update = "industrialPart:update";
        public const string Delete = "industrialPart:delete";
    }

    public static class MedicalProgram
    {
        public const int CreateId = 48;
        public const int ReadId = 49;
        public const int UpdateId = 50;
        public const int DeleteId = 51;

        public const string Create = "medicalProgram:create";
        public const string Read = "medicalProgram:read";
        public const string Update = "medicalProgram:update";
        public const string Delete = "medicalProgram:delete";
    }

    public static class Department
    {
        public const int CreateId = 52;
        public const int ReadId = 53;
        public const int UpdateId = 54;
        public const int DeleteId = 55;

        public const string Create = "department:create";
        public const string Read = "department:read";
        public const string Update = "department:update";
        public const string Delete = "department:delete";
    }

    public static class Section
    {
        public const int CreateId = 56;
        public const int ReadId = 57;
        public const int UpdateId = 58;
        public const int DeleteId = 59;

        public const string Create = "section:create";
        public const string Read = "section:read";
        public const string Update = "section:update";
        public const string Delete = "section:delete";
    }

    public static class Room
    {
        public const int CreateId = 60;
        public const int ReadId = 61;
        public const int UpdateId = 62;
        public const int DeleteId = 63;

        public const string Create = "room:create";
        public const string Read = "room:read";
        public const string Update = "room:update";
        public const string Delete = "room:delete";
    }

    public static class Payment
    {
        public const int CreateId = 64;
        public const int ReadId = 65;
        public const int UpdateId = 66;
        public const int DeleteId = 67;
        public const int PrintInvoiceId = 68;

        public const string Create = "payment:create";
        public const string Read = "payment:read";
        public const string Update = "payment:update";
        public const string Delete = "payment:delete";
        public const string PrintInvoice = "payment:print_invoice";
    }

    public static class Doctor
    {
        public const int CreateId = 69;
        public const int ReadId = 70;
        public const int UpdateId = 71;
        public const int DeleteId = 72;
        public const int AssignDoctorToSectionId = 73;
        public const int AssignDoctorToSectionAndRoomId = 74;
        public const int ChangeDoctorDepartmentId = 75;
        public const int EndDoctorAssignmentId = 76;

        public const string Create = "doctor:create";
        public const string Read = "doctor:read";
        public const string Update = "doctor:update";
        public const string Delete = "doctor:delete";
        public const string AssignDoctorToSection = "doctor:assign_doctor_to_section";
        public const string AssignDoctorToSectionAndRoom = "doctor:assign_doctor_to_section_and_room";
        public const string ChangeDoctorDepartment = "doctor:change_doctor_department";
        public const string EndDoctorAssignment = "doctor:end_doctor_assignment";
    }

    public static class Patient
    {
        public const int CreateId = 77;
        public const int ReadId = 78;
        public const int UpdateId = 79;
        public const int DeleteId = 80;

        public const string Create = "patient:create";
        public const string Read = "patient:read";
        public const string Update = "patient:update";
        public const string Delete = "patient:delete";
    }

    public static class DisabledCard
    {
        public const int CreateId = 81;
        public const int ReadId = 82;
        public const int UpdateId = 83;
        public const int DeleteId = 84;

        public const string Create = "disabledCard:create";
        public const string Read = "disabledCard:read";
        public const string Update = "disabledCard:update";
        public const string Delete = "disabledCard:delete";
    }

    public static class Sale
    {
        public const int CreateId = 85;
        public const int ReadId = 86;
        public const int UpdateId = 87;
        public const int DeleteId = 88;
        public const int ChangeStatusId = 89;

        public const string Create = "sale:create";
        public const string Read = "sale:read";
        public const string Update = "sale:update";
        public const string Delete = "sale:delete";
        public const string ChangeStatus = "sale:change_status";
    }

    public static class ExitCard
    {
        public const int CreateId = 90;
        public const int ReadId = 91;
        public const int UpdateId = 92;
        public const int DeleteId = 93;
        public const int PrintId = 94;

        public const string Create = "exitCard:create";
        public const string Read = "exitCard:read";
        public const string Update = "exitCard:update";
        public const string Delete = "exitCard:delete";
        public const string Print = "exitCard:print";
    }

    public static class User
    {
        public const int CreateId = 95;
        public const int ReadId = 96;
        public const int UpdateId = 97;
        public const int DeleteId = 98;
        public const int GrantPermissionsId = 99;
        public const int DenyPermissionsId = 100;
        public const int AssignRolesId = 101;
        public const int RemoveRolesId = 102;

        public const string Create = "user:create";
        public const string Read = "user:read";
        public const string Update = "user:update";
        public const string Delete = "user:delete";
        public const string GrantPermissions = "user:grant_permissions";
        public const string DenyPermissions = "user:deny_permissions";
        public const string AssignRoles = "user:assign_roles";
        public const string RemoveRoles = "user:remove_roles";
    }


    public static class Role
    {
        public const int ReadId = 103;
        public const int ActivatePermissionsId = 104;
        public const int DeactivatePermissionsId = 105;

        public const string Read = "role:read";
        public const string ActivatePermissions = "role:activate_permissions";
        public const string DeactivatePermissions = "role:deactivate_permissions";
    }


    public static class Order
    {
        public const int CreateId = 106;
        public const int ReadId = 107;
        public const int UpdateId = 108;
        public const int DeleteId = 109;
        public const int ChangeStatusId = 110;
        public const int PrintId = 111;

        public const string Create = "order:create";
        public const string Read = "order:read";
        public const string Update = "order:update";
        public const string Delete = "order:delete";
        public const string ChangeStatus = "order:change_status";
        public const string Print = "order:print";
    }

    public static class ExchangeOrder
    {
        public const int CreateId = 112;
        public const int ReadId = 113;
        public const int UpdateId = 114;
        public const int DeleteId = 115;
        public const int ChangeStatusId = 116;
        public const int PrintId = 117;

        public const string Create = "exchangeOrder:create";
        public const string Read = "exchangeOrder:read";
        public const string Update = "exchangeOrder:update";
        public const string Delete = "exchangeOrder:delete";
        public const string ChangeStatus = "exchangeOrder:change_status";
        public const string Print = "exchangeOrder:print";
    }

    public static class Purchase
    {
        public const int CreateId = 118;
        public const int ReadId = 119;
        public const int UpdateId = 120;
        public const int DeleteId = 121;
        public const int ChangeStatusId = 122;

        public const string Create = "purchase:create";
        public const string Read = "purchase:read";
        public const string Update = "purchase:update";
        public const string Delete = "purchase:delete";
        public const string ChangeStatus = "purchase:change_status";
    }
}

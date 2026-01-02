namespace AlatrafClinic.Domain.Common.Constants;

public static class Permission
{
    public static class Person
    {
        public const string Create = "person:create";
        public const string Read = "person:read";
        public const string Update = "person:update";
        public const string Delete = "person:delete";

    }

    public static class Service
    {
        public const string Create = "service:create";
        public const string Read = "service:read";
        public const string Update = "service:update";
        public const string Delete = "service:delete";
    }

    public static class Ticket
    {
        public const string Create = "ticket:create";
        public const string Read = "ticket:read";
        public const string Update = "ticket:update";
        public const string Delete = "ticket:delete";
        public const string Print = "ticket:print";
    }

    public static class Appointment
    {
        public const string Create = "appointment:create";
        public const string ReSchedule = "appointment:reschedule";
        public const string Read = "appointment:read";
        public const string Update = "appointment:update";
        public const string Delete = "appointment:delete";
        public const string ChangeStatus = "appointment:change_status";
    }
    public static class Holiday
    {
        public const string Create = "holiday:create";
        public const string Read = "holiday:read";
        public const string Update = "holiday:update";
        public const string Delete = "holiday:delete";
    }

    public static class TherapyCard
    {
        public const string Create = "therapyCard:create";
        public const string Read = "therapyCard:read";
        public const string Update = "therapyCard:update";
        public const string Delete = "therapyCard:delete";
        public const string Renew = "therapyCard:renew";
        public const string CreateSession = "therapyCard:create_session";
    }

    public static class RepairCard
    {
        public const string Create = "repairCard:create";
        public const string Read = "repairCard:read";
        public const string Update = "repairCard:update";
        public const string Delete = "repairCard:delete";
        public const string ChangeStatus = "repairCard:change_status";
        public const string AssignToTechnician = "repairCard:assign_to_technician";
        public const string CreateDeliveryTime = "repairCard:create_delivery_time";
    }

    public static class IndustrialPart
    {
        public const string Create = "industrialPart:create";
        public const string Read = "industrialPart:read";
        public const string Update = "industrialPart:update";
        public const string Delete = "industrialPart:delete";
    }
    public static class MedicalProgram
    {
        public const string Create = "medicalProgram:create";
        public const string Read = "medicalProgram:read";
        public const string Update = "medicalProgram:update";
        public const string Delete = "medicalProgram:delete";
    }
    public static class Department
    {
        public const string Create = "department:create";
        public const string Read = "department:read";
        public const string Update = "department:update";
        public const string Delete = "department:delete";
    }
    public static class Section
    {
        public const string Create = "section:create";
        public const string Read = "section:read";
        public const string Update = "section:update";
        public const string Delete = "section:delete";
    }
    public static class Room
    {
        public const string Create = "room:create";
        public const string Read = "room:read";
        public const string Update = "room:update";
        public const string Delete = "room:delete";
    }

    public static class Payment
    {
        public const string Create = "payment:create";
        public const string Read = "payment:read";
        public const string Update = "payment:update";
        public const string Delete = "payment:delete";
    }
    public static class Doctor
    {
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
        public const string Create = "patient:create";
        public const string Read = "patient:read";
        public const string Update = "patient:update";
        public const string Delete = "patient:delete";
    }
    public static class DisabledCard
    {
        public const string Read = "disabledCard:read";
        public const string Create = "disabledCard:create";
        public const string Update = "disabledCard:update";
        public const string Delete = "disabledCard:delete";
    }

    public static class Sale
    {
        public const string Create = "sale:create";
        public const string Read = "sale:read";
        public const string Update = "sale:update";
        public const string Delete = "sale:delete";
        public const string Cancel = "sale:change_status";
    }
    public static class User
    {
        public const string Create = "user:create";
        public const string Read = "user:read";
        public const string Update = "user:update";
        public const string Delete = "user:delete";
        public const string GrantPermissions = "user:grant_permissions";
        public const string DenyPermissions = "user:deny_permissions";
        public const string RemovePermissionOverrides = "user:remove_permission_overrides";
        public const string AssignRoles = "user:assign_roles";
        public const string RemoveRoles = "user:remove_roles";
    }
    
    public static class Role
    {
        public const string Create = "role:create";
        public const string Read = "role:read";
        public const string Update = "role:update";
        public const string Delete = "role:delete";
        public const string AssignPermissions = "role:assign_permissions";
        public const string RemovePermissions = "role:remove_permissions";
    }
}
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Departments;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Identity;
using AlatrafClinic.Domain.Inventory.Units;
using AlatrafClinic.Domain.People;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;
using AlatrafClinic.Domain.Reports;
using AlatrafClinic.Domain.Services;
using AlatrafClinic.Domain.Settings;
using AlatrafClinic.Domain.TherapyCards.Enums;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;
using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;
using AlatrafClinic.Infrastructure.Identity;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace AlatrafClinic.Infrastructure.Data;

public sealed class AlatrafClinicDbContextInitialiser
{
    private readonly ILogger<AlatrafClinicDbContextInitialiser> _logger;
    private readonly AlatrafClinicDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public AlatrafClinicDbContextInitialiser(
        ILogger<AlatrafClinicDbContextInitialiser> logger,
        AlatrafClinicDbContext context,
        RoleManager<IdentityRole> roleManager,
        UserManager<AppUser> userManager
        )
    {
        _logger = logger;
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task InitialiseAsync(CancellationToken ct = default)
    {
        try
        {   
            await _context.Database.MigrateAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
    private async Task TrySeedAsync()
    {
        if (!await _context.Permissions.AnyAsync())
        {
            _context.Permissions.AddRange(
                // Person
                new ApplicationPermission { Id = Permission.Person.CreateId, Name = Permission.Person.Create },
                new ApplicationPermission { Id = Permission.Person.ReadId, Name = Permission.Person.Read },
                new ApplicationPermission { Id = Permission.Person.UpdateId, Name = Permission.Person.Update },
                new ApplicationPermission { Id = Permission.Person.DeleteId, Name = Permission.Person.Delete },

                // Service
                new ApplicationPermission { Id = Permission.Service.CreateId, Name = Permission.Service.Create },
                new ApplicationPermission { Id = Permission.Service.ReadId, Name = Permission.Service.Read },
                new ApplicationPermission { Id = Permission.Service.UpdateId, Name = Permission.Service.Update },
                new ApplicationPermission { Id = Permission.Service.DeleteId, Name = Permission.Service.Delete },

                // Ticket
                new ApplicationPermission { Id = Permission.Ticket.CreateId, Name = Permission.Ticket.Create },
                new ApplicationPermission { Id = Permission.Ticket.ReadId, Name = Permission.Ticket.Read },
                new ApplicationPermission { Id = Permission.Ticket.UpdateId, Name = Permission.Ticket.Update },
                new ApplicationPermission { Id = Permission.Ticket.DeleteId, Name = Permission.Ticket.Delete },
                new ApplicationPermission { Id = Permission.Ticket.PrintId, Name = Permission.Ticket.Print },

                // Appointment
                new ApplicationPermission { Id = Permission.Appointment.CreateId, Name = Permission.Appointment.Create },
                new ApplicationPermission { Id = Permission.Appointment.ReScheduleId, Name = Permission.Appointment.ReSchedule },
                new ApplicationPermission { Id = Permission.Appointment.ReadId, Name = Permission.Appointment.Read },
                new ApplicationPermission { Id = Permission.Appointment.UpdateId, Name = Permission.Appointment.Update },
                new ApplicationPermission { Id = Permission.Appointment.DeleteId, Name = Permission.Appointment.Delete },
                new ApplicationPermission { Id = Permission.Appointment.ChangeStatusId, Name = Permission.Appointment.ChangeStatus },
                new ApplicationPermission { Id = Permission.Appointment.PrintId, Name = Permission.Appointment.Print },

                // Holiday
                new ApplicationPermission { Id = Permission.Holiday.CreateId, Name = Permission.Holiday.Create },
                new ApplicationPermission { Id = Permission.Holiday.ReadId, Name = Permission.Holiday.Read },
                new ApplicationPermission { Id = Permission.Holiday.UpdateId, Name = Permission.Holiday.Update },
                new ApplicationPermission { Id = Permission.Holiday.DeleteId, Name = Permission.Holiday.Delete },

                // TherapyCard
                new ApplicationPermission { Id = Permission.TherapyCard.CreateId, Name = Permission.TherapyCard.Create },
                new ApplicationPermission { Id = Permission.TherapyCard.ReadId, Name = Permission.TherapyCard.Read },
                new ApplicationPermission { Id = Permission.TherapyCard.UpdateId, Name = Permission.TherapyCard.Update },
                new ApplicationPermission { Id = Permission.TherapyCard.DeleteId, Name = Permission.TherapyCard.Delete },
                new ApplicationPermission { Id = Permission.TherapyCard.RenewId, Name = Permission.TherapyCard.Renew },
                new ApplicationPermission { Id = Permission.TherapyCard.CreateSessionId, Name = Permission.TherapyCard.CreateSession },
                new ApplicationPermission { Id = Permission.TherapyCard.PrintTherapyCardId, Name = Permission.TherapyCard.PrintTherapyCard },
                new ApplicationPermission { Id = Permission.TherapyCard.PrintSessionId, Name = Permission.TherapyCard.PrintSession },
                new ApplicationPermission { Id = Permission.TherapyCard.DamageReplacementId, Name = Permission.TherapyCard.DamageReplacement },
                new ApplicationPermission { Id = Permission.TherapyCard.ReadSessionId, Name = Permission.TherapyCard.ReadSession },
                // RepairCard
                new ApplicationPermission { Id = Permission.RepairCard.CreateId, Name = Permission.RepairCard.Create },
                new ApplicationPermission { Id = Permission.RepairCard.ReadId, Name = Permission.RepairCard.Read },
                new ApplicationPermission { Id = Permission.RepairCard.UpdateId, Name = Permission.RepairCard.Update },
                new ApplicationPermission { Id = Permission.RepairCard.DeleteId, Name = Permission.RepairCard.Delete },
                new ApplicationPermission { Id = Permission.RepairCard.ChangeStatusId, Name = Permission.RepairCard.ChangeStatus },
                new ApplicationPermission { Id = Permission.RepairCard.AssignToTechnicianId, Name = Permission.RepairCard.AssignToTechnician },
                new ApplicationPermission { Id = Permission.RepairCard.CreateDeliveryTimeId, Name = Permission.RepairCard.CreateDeliveryTime },
                new ApplicationPermission { Id = Permission.RepairCard.PrintRepairCardId, Name = Permission.RepairCard.PrintRepairCard },
                new ApplicationPermission { Id = Permission.RepairCard.PrintDeliveryTimeId, Name = Permission.RepairCard.PrintDeliveryTime },

                // IndustrialPart
                new ApplicationPermission { Id = Permission.IndustrialPart.CreateId, Name = Permission.IndustrialPart.Create },
                new ApplicationPermission { Id = Permission.IndustrialPart.ReadId, Name = Permission.IndustrialPart.Read },
                new ApplicationPermission { Id = Permission.IndustrialPart.UpdateId, Name = Permission.IndustrialPart.Update },
                new ApplicationPermission { Id = Permission.IndustrialPart.DeleteId, Name = Permission.IndustrialPart.Delete },

                // MedicalProgram
                new ApplicationPermission { Id = Permission.MedicalProgram.CreateId, Name = Permission.MedicalProgram.Create },
                new ApplicationPermission { Id = Permission.MedicalProgram.ReadId, Name = Permission.MedicalProgram.Read },
                new ApplicationPermission { Id = Permission.MedicalProgram.UpdateId, Name = Permission.MedicalProgram.Update },
                new ApplicationPermission { Id = Permission.MedicalProgram.DeleteId, Name = Permission.MedicalProgram.Delete },

                // Department
                new ApplicationPermission { Id = Permission.Department.CreateId, Name = Permission.Department.Create },
                new ApplicationPermission { Id = Permission.Department.ReadId, Name = Permission.Department.Read },
                new ApplicationPermission { Id = Permission.Department.UpdateId, Name = Permission.Department.Update },
                new ApplicationPermission { Id = Permission.Department.DeleteId, Name = Permission.Department.Delete },

                // Section
                new ApplicationPermission { Id = Permission.Section.CreateId, Name = Permission.Section.Create },
                new ApplicationPermission { Id = Permission.Section.ReadId, Name = Permission.Section.Read },
                new ApplicationPermission { Id = Permission.Section.UpdateId, Name = Permission.Section.Update },
                new ApplicationPermission { Id = Permission.Section.DeleteId, Name = Permission.Section.Delete },

                // Room
                new ApplicationPermission { Id = Permission.Room.CreateId, Name = Permission.Room.Create },
                new ApplicationPermission { Id = Permission.Room.ReadId, Name = Permission.Room.Read },
                new ApplicationPermission { Id = Permission.Room.UpdateId, Name = Permission.Room.Update },
                new ApplicationPermission { Id = Permission.Room.DeleteId, Name = Permission.Room.Delete },

                // Payment
                new ApplicationPermission { Id = Permission.Payment.CreateId, Name = Permission.Payment.Create },
                new ApplicationPermission { Id = Permission.Payment.ReadId, Name = Permission.Payment.Read },
                new ApplicationPermission { Id = Permission.Payment.UpdateId, Name = Permission.Payment.Update },
                new ApplicationPermission { Id = Permission.Payment.DeleteId, Name = Permission.Payment.Delete },
                new ApplicationPermission { Id = Permission.Payment.PrintInvoiceId, Name = Permission.Payment.PrintInvoice },

                // Doctor
                new ApplicationPermission { Id = Permission.Doctor.CreateId, Name = Permission.Doctor.Create },
                new ApplicationPermission { Id = Permission.Doctor.ReadId, Name = Permission.Doctor.Read },
                new ApplicationPermission { Id = Permission.Doctor.UpdateId, Name = Permission.Doctor.Update },
                new ApplicationPermission { Id = Permission.Doctor.DeleteId, Name = Permission.Doctor.Delete },
                new ApplicationPermission { Id = Permission.Doctor.AssignDoctorToSectionId, Name = Permission.Doctor.AssignDoctorToSection },
                new ApplicationPermission { Id = Permission.Doctor.AssignDoctorToSectionAndRoomId, Name = Permission.Doctor.AssignDoctorToSectionAndRoom },
                new ApplicationPermission { Id = Permission.Doctor.ChangeDoctorDepartmentId, Name = Permission.Doctor.ChangeDoctorDepartment },
                new ApplicationPermission { Id = Permission.Doctor.EndDoctorAssignmentId, Name = Permission.Doctor.EndDoctorAssignment },

                // Patient
                new ApplicationPermission { Id = Permission.Patient.CreateId, Name = Permission.Patient.Create },
                new ApplicationPermission { Id = Permission.Patient.ReadId, Name = Permission.Patient.Read },
                new ApplicationPermission { Id = Permission.Patient.UpdateId, Name = Permission.Patient.Update },
                new ApplicationPermission { Id = Permission.Patient.DeleteId, Name = Permission.Patient.Delete },

                // DisabledCard
                new ApplicationPermission { Id = Permission.DisabledCard.CreateId, Name = Permission.DisabledCard.Create },
                new ApplicationPermission { Id = Permission.DisabledCard.ReadId, Name = Permission.DisabledCard.Read },
                new ApplicationPermission { Id = Permission.DisabledCard.UpdateId, Name = Permission.DisabledCard.Update },
                new ApplicationPermission { Id = Permission.DisabledCard.DeleteId, Name = Permission.DisabledCard.Delete },

                // Sale
                new ApplicationPermission { Id = Permission.Sale.CreateId, Name = Permission.Sale.Create },
                new ApplicationPermission { Id = Permission.Sale.ReadId, Name = Permission.Sale.Read },
                new ApplicationPermission { Id = Permission.Sale.UpdateId, Name = Permission.Sale.Update },
                new ApplicationPermission { Id = Permission.Sale.DeleteId, Name = Permission.Sale.Delete },
                new ApplicationPermission { Id = Permission.Sale.ChangeStatusId, Name = Permission.Sale.ChangeStatus },

                // User
                new ApplicationPermission { Id = Permission.User.CreateId, Name = Permission.User.Create },
                new ApplicationPermission { Id = Permission.User.ReadId, Name = Permission.User.Read },
                new ApplicationPermission { Id = Permission.User.UpdateId, Name = Permission.User.Update },
                new ApplicationPermission { Id = Permission.User.DeleteId, Name = Permission.User.Delete },
                new ApplicationPermission { Id = Permission.User.GrantPermissionsId, Name = Permission.User.GrantPermissions },
                new ApplicationPermission { Id = Permission.User.DenyPermissionsId, Name = Permission.User.DenyPermissions },
                new ApplicationPermission { Id = Permission.User.AssignRolesId, Name = Permission.User.AssignRoles },
                new ApplicationPermission { Id = Permission.User.RemoveRolesId, Name = Permission.User.RemoveRoles },

                // Role
                new ApplicationPermission { Id = Permission.Role.ReadId, Name = Permission.Role.Read },
                new ApplicationPermission { Id = Permission.Role.ActivatePermissionsId, Name = Permission.Role.ActivatePermissions },
                new ApplicationPermission { Id = Permission.Role.DeactivatePermissionsId, Name = Permission.Role.DeactivatePermissions },
                // Exit Card
                new ApplicationPermission { Id = Permission.ExitCard.CreateId, Name = Permission.ExitCard.Create },
                new ApplicationPermission { Id = Permission.ExitCard.ReadId, Name = Permission.ExitCard.Read },
                new ApplicationPermission { Id = Permission.ExitCard.UpdateId, Name = Permission.ExitCard.Update
                },
                new ApplicationPermission { Id = Permission.ExitCard.DeleteId, Name = Permission.ExitCard.Delete },
                new ApplicationPermission { Id = Permission.ExitCard.PrintId, Name = Permission.ExitCard.Print },
                // Orders
                new ApplicationPermission { Id = Permission.Order.CreateId, Name = Permission.Order.Create },
                new ApplicationPermission { Id = Permission.Order.ReadId, Name = Permission.Order.Read },
                new ApplicationPermission { Id = Permission.Order.UpdateId, Name = Permission.Order.Update },
                new ApplicationPermission { Id = Permission.Order.DeleteId, Name = Permission.Order.Delete },
                new ApplicationPermission { Id = Permission.Order.ChangeStatusId, Name = Permission.Order.ChangeStatus },
                new ApplicationPermission { Id = Permission.Order.PrintId, Name = Permission.Order.Print },
                // Exchange Orders
                new ApplicationPermission { Id = Permission.ExchangeOrder.CreateId, Name = Permission.ExchangeOrder.Create },
                new ApplicationPermission { Id = Permission.ExchangeOrder.ReadId, Name = Permission.ExchangeOrder.Read },
                new ApplicationPermission { Id = Permission.ExchangeOrder.UpdateId, Name = Permission.ExchangeOrder.Update },
                new ApplicationPermission { Id = Permission.ExchangeOrder.DeleteId, Name = Permission.ExchangeOrder.Delete },
                new ApplicationPermission { Id = Permission.ExchangeOrder.ChangeStatusId, Name = Permission.ExchangeOrder.ChangeStatus },
                new ApplicationPermission { Id = Permission.ExchangeOrder.PrintId, Name = Permission.ExchangeOrder.Print },
                // Purchases
                new ApplicationPermission { Id = Permission.Purchase.CreateId, Name = Permission.Purchase.Create },
                new ApplicationPermission { Id = Permission.Purchase.ReadId, Name = Permission.Purchase.Read },
                new ApplicationPermission { Id = Permission.Purchase.UpdateId, Name = Permission.Purchase.Update },
                new ApplicationPermission { Id = Permission.Purchase.DeleteId, Name = Permission.Purchase.Delete },
                new ApplicationPermission { Id = Permission.Purchase.ChangeStatusId, Name = Permission.Purchase.ChangeStatus }
            );
            await _context.SaveChangesAsync();
        }
        
       
        // Default roles
        var adminRole = new IdentityRole(nameof(Role.Admin));
        var receptionistRole = new IdentityRole(nameof(Role.Receptionist));
        var appointmentsEmployeeRole = new IdentityRole(nameof(Role.AppointmentsEmployee));
        var financeEmployeeRole = new IdentityRole(nameof(Role.FinanceEmployee));
        var therapyDoctorRole = new IdentityRole(nameof(Role.TherapyDoctor));
        var industrialDoctorRole = new IdentityRole(nameof(Role.IndustrialDoctor));
        var technicalManagementReceptionistRole = new IdentityRole(nameof(Role.TechnicalManagementReceptionist));
        var therapyManagementReceptionistRole = new IdentityRole(nameof(Role.TherapyManagementReceptionist));
        var ordersEmployeeRole = new IdentityRole(nameof(Role.OrdersEmployee));
        var exchangeOrderEmployeeRole = new IdentityRole(nameof(Role.ExchangeOrderEmployee));
        var salesEmployeeRole = new IdentityRole(nameof(Role.SalesEmployee));
        var purchaseEmployeeRole = new IdentityRole(nameof(Role.PurchaseEmployee));
        var exitsEmployeeRole = new IdentityRole(nameof(Role.ExitsEmployee));

        if (!await _roleManager.RoleExistsAsync(adminRole.Name!))
        {
            var result = await _roleManager.CreateAsync(adminRole);
             // 3. ASSIGN PERMISSIONS TO ROLE - RIGHT AFTER CREATING THE ROLE
            var allPermissions = await _context.Permissions.ToListAsync();
            foreach (var permission in allPermissions)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id,
                    IsActive = true
                });
            }
            await _context.SaveChangesAsync();     
        }

        if (!await _roleManager.RoleExistsAsync(receptionistRole.Name!))
        {
            await _roleManager.CreateAsync(receptionistRole);
            _context.RolePermissions.AddRange(
                
                new RolePermission { RoleId = receptionistRole.Id, PermissionId = Permission.Patient.ReadId, IsActive = true },
                new RolePermission { RoleId = receptionistRole.Id, PermissionId = Permission.Patient.CreateId, IsActive = true },
                new RolePermission { RoleId = receptionistRole.Id, PermissionId = Permission.Patient.UpdateId, IsActive = true },
                new RolePermission { RoleId = receptionistRole.Id, PermissionId = Permission.Patient.DeleteId, IsActive = true },

                new RolePermission { RoleId = receptionistRole.Id, PermissionId = Permission.Ticket.ReadId, IsActive = true },
                new RolePermission { RoleId = receptionistRole.Id, PermissionId = Permission.Ticket.CreateId, IsActive = true },
                new RolePermission { RoleId = receptionistRole.Id, PermissionId = Permission.Ticket.UpdateId, IsActive = true },
                new RolePermission { RoleId = receptionistRole.Id, PermissionId = Permission.Ticket.DeleteId, IsActive = true },
                new RolePermission { RoleId = receptionistRole.Id, PermissionId = Permission.Ticket.PrintId, IsActive = true },

                new RolePermission { RoleId = receptionistRole.Id, PermissionId = Permission.Service.ReadId, IsActive = true },

                new RolePermission { RoleId = receptionistRole.Id, PermissionId = Permission.TherapyCard.ReadId, IsActive = true },
                
                new RolePermission { RoleId = receptionistRole.Id, PermissionId = Permission.TherapyCard.DamageReplacementId, IsActive = true }
            );
        }

        if (!await _roleManager.RoleExistsAsync(appointmentsEmployeeRole.Name!))
        {
            await _roleManager.CreateAsync(appointmentsEmployeeRole);
            _context.RolePermissions.AddRange(
                
                new RolePermission { RoleId = appointmentsEmployeeRole.Id, PermissionId = Permission.Appointment.ReadId, IsActive = true },
                new RolePermission { RoleId = appointmentsEmployeeRole.Id, PermissionId = Permission.Appointment.CreateId, IsActive = true },
                new RolePermission { RoleId = appointmentsEmployeeRole.Id, PermissionId = Permission.Appointment.UpdateId, IsActive = true },
                new RolePermission { RoleId = appointmentsEmployeeRole.Id, PermissionId = Permission.Appointment.DeleteId, IsActive = true },
                new RolePermission { RoleId = appointmentsEmployeeRole.Id, PermissionId = Permission.Appointment.ChangeStatusId, IsActive = true },
                new RolePermission { RoleId = appointmentsEmployeeRole.Id, PermissionId = Permission.Appointment.PrintId, IsActive = true },
                
                new RolePermission { RoleId = appointmentsEmployeeRole.Id, PermissionId = Permission.Ticket.ReadId, IsActive = true }
            );
        }

        

        if (!await _roleManager.RoleExistsAsync(therapyDoctorRole.Name!))
        {
            await _roleManager.CreateAsync(therapyDoctorRole);
            _context.RolePermissions.AddRange(
                
                new RolePermission { RoleId = therapyDoctorRole.Id, PermissionId = Permission.TherapyCard.ReadId, IsActive = true },
                new RolePermission { RoleId = therapyDoctorRole.Id, PermissionId = Permission.TherapyCard.CreateId, IsActive = true },
                new RolePermission { RoleId = therapyDoctorRole.Id, PermissionId = Permission.TherapyCard.UpdateId, IsActive = true },
                new RolePermission { RoleId = therapyDoctorRole.Id, PermissionId = Permission.TherapyCard.DeleteId, IsActive = true },
                new RolePermission { RoleId = therapyDoctorRole.Id, PermissionId = Permission.TherapyCard.RenewId, IsActive = true },
                new RolePermission { RoleId = therapyDoctorRole.Id, PermissionId = Permission.Ticket.ReadId, IsActive = true }
            );
        }

        if (!await _roleManager.RoleExistsAsync(industrialDoctorRole.Name!))
        {
            await _roleManager.CreateAsync(industrialDoctorRole);
            _context.RolePermissions.AddRange(
                
                new RolePermission { RoleId = industrialDoctorRole.Id, PermissionId = Permission.RepairCard.ReadId, IsActive = true },
                new RolePermission { RoleId = industrialDoctorRole.Id, PermissionId = Permission.RepairCard.CreateId, IsActive = true },
                new RolePermission { RoleId = industrialDoctorRole.Id, PermissionId = Permission.RepairCard.UpdateId, IsActive = true },
                new RolePermission { RoleId = industrialDoctorRole.Id, PermissionId = Permission.RepairCard.DeleteId, IsActive = true },
                
                new RolePermission { RoleId = industrialDoctorRole.Id, PermissionId = Permission.Ticket.ReadId, IsActive = true }
            );
        }

        if (!await _roleManager.RoleExistsAsync(financeEmployeeRole.Name!))
        {
            await _roleManager.CreateAsync(financeEmployeeRole);
            _context.RolePermissions.AddRange(
                
                new RolePermission { RoleId = financeEmployeeRole.Id, PermissionId = Permission.Payment.ReadId, IsActive = true },
                new RolePermission { RoleId = financeEmployeeRole.Id, PermissionId = Permission.Payment.CreateId, IsActive = true },
                new RolePermission { RoleId = financeEmployeeRole.Id, PermissionId = Permission.Payment.UpdateId, IsActive = true },
                new RolePermission { RoleId = financeEmployeeRole.Id, PermissionId = Permission.Payment.DeleteId, IsActive = true }
            );
        }

        if (!await _roleManager.RoleExistsAsync(therapyManagementReceptionistRole.Name!))
        {
            await _roleManager.CreateAsync(therapyManagementReceptionistRole);
            _context.RolePermissions.AddRange(

                new RolePermission { RoleId = therapyManagementReceptionistRole.Id, PermissionId = Permission.TherapyCard.ReadSessionId, IsActive = true },
                new RolePermission { RoleId = therapyManagementReceptionistRole.Id, PermissionId = Permission.TherapyCard.CreateSessionId, IsActive = true },
                new RolePermission { RoleId = therapyManagementReceptionistRole.Id, PermissionId = Permission.TherapyCard.PrintSessionId, IsActive = true },
                new RolePermission { RoleId = therapyManagementReceptionistRole.Id, PermissionId = Permission.TherapyCard.PrintTherapyCardId, IsActive = true }
                
            );
        }

        if (!await _roleManager.RoleExistsAsync(technicalManagementReceptionistRole.Name!))
        {
            await _roleManager.CreateAsync(technicalManagementReceptionistRole);
            _context.RolePermissions.AddRange(
                
                new RolePermission { RoleId = technicalManagementReceptionistRole.Id, PermissionId = Permission.RepairCard.AssignToTechnicianId, IsActive = true },
                new RolePermission { RoleId = technicalManagementReceptionistRole.Id, PermissionId = Permission.RepairCard.CreateDeliveryTimeId, IsActive = true },
                new RolePermission { RoleId = technicalManagementReceptionistRole.Id, PermissionId = Permission.RepairCard.PrintRepairCardId, IsActive = true },
                new RolePermission { RoleId = technicalManagementReceptionistRole.Id, PermissionId = Permission.RepairCard.PrintDeliveryTimeId, IsActive = true }    
            );
        }
        if (!await _roleManager.RoleExistsAsync(ordersEmployeeRole.Name!))
        {
            await _roleManager.CreateAsync(ordersEmployeeRole);
            _context.RolePermissions.AddRange(
                
                new RolePermission { RoleId = ordersEmployeeRole.Id, PermissionId = Permission.Order.ReadId, IsActive = true },
                new RolePermission { RoleId = ordersEmployeeRole.Id, PermissionId = Permission.Order.CreateId, IsActive = true },
                new RolePermission { RoleId = ordersEmployeeRole.Id, PermissionId = Permission.Order.UpdateId, IsActive = true },
                new RolePermission { RoleId = ordersEmployeeRole.Id, PermissionId = Permission.Order.DeleteId, IsActive = true },
                new RolePermission { RoleId = ordersEmployeeRole.Id, PermissionId = Permission.Order.ChangeStatusId, IsActive = true },
                new RolePermission { RoleId = ordersEmployeeRole.Id, PermissionId = Permission.Order.PrintId, IsActive = true }    
            );
        }

        if (!await _roleManager.RoleExistsAsync(exchangeOrderEmployeeRole.Name!))
        {
            await _roleManager.CreateAsync(exchangeOrderEmployeeRole);
            _context.RolePermissions.AddRange(
                
                new RolePermission { RoleId = exchangeOrderEmployeeRole.Id, PermissionId = Permission.ExchangeOrder.ReadId, IsActive = true },
                new RolePermission { RoleId = exchangeOrderEmployeeRole.Id, PermissionId = Permission.ExchangeOrder.CreateId, IsActive = true },
                new RolePermission { RoleId = exchangeOrderEmployeeRole.Id, PermissionId = Permission.ExchangeOrder.UpdateId, IsActive = true },
                new RolePermission { RoleId = exchangeOrderEmployeeRole.Id, PermissionId = Permission.ExchangeOrder.DeleteId, IsActive = true },
                new RolePermission { RoleId = exchangeOrderEmployeeRole.Id, PermissionId = Permission.ExchangeOrder.ChangeStatusId, IsActive = true },
                new RolePermission { RoleId = exchangeOrderEmployeeRole.Id, PermissionId = Permission.ExchangeOrder.PrintId, IsActive = true }    
            );
        }
        if (!await _roleManager.RoleExistsAsync(salesEmployeeRole.Name!))
        {
            await _roleManager.CreateAsync(salesEmployeeRole);
            _context.RolePermissions.AddRange(
                
                new RolePermission { RoleId = salesEmployeeRole.Id, PermissionId = Permission.Sale.ReadId, IsActive = true },
                new RolePermission { RoleId = salesEmployeeRole.Id, PermissionId = Permission.Sale.CreateId, IsActive = true },
                new RolePermission { RoleId = salesEmployeeRole.Id, PermissionId = Permission.Sale.UpdateId, IsActive = true },
                new RolePermission { RoleId = salesEmployeeRole.Id, PermissionId = Permission.Sale.DeleteId, IsActive = true },
                new RolePermission { RoleId = salesEmployeeRole.Id, PermissionId = Permission.Sale.ChangeStatusId, IsActive = true }   
            );
        }
        if (!await _roleManager.RoleExistsAsync(purchaseEmployeeRole.Name!))
        {
            await _roleManager.CreateAsync(purchaseEmployeeRole);
            _context.RolePermissions.AddRange(
                
                new RolePermission { RoleId = purchaseEmployeeRole.Id, PermissionId = Permission.Purchase.ReadId, IsActive = true },
                new RolePermission { RoleId = purchaseEmployeeRole.Id, PermissionId = Permission.Purchase.CreateId, IsActive = true },
                new RolePermission { RoleId = purchaseEmployeeRole.Id, PermissionId = Permission.Purchase.UpdateId, IsActive = true },
                new RolePermission { RoleId = purchaseEmployeeRole.Id, PermissionId = Permission.Purchase.DeleteId, IsActive = true },
                new RolePermission { RoleId = purchaseEmployeeRole.Id, PermissionId = Permission.Purchase.ChangeStatusId, IsActive = true }   
            );
        }
        
        if (!await _roleManager.RoleExistsAsync(exitsEmployeeRole.Name!))
        {
            await _roleManager.CreateAsync(exitsEmployeeRole);
            _context.RolePermissions.AddRange(
                
                new RolePermission { RoleId = exitsEmployeeRole.Id, PermissionId = Permission.ExitCard.ReadId, IsActive = true },
                new RolePermission { RoleId = exitsEmployeeRole.Id, PermissionId = Permission.ExitCard.CreateId, IsActive = true },
                new RolePermission { RoleId = exitsEmployeeRole.Id, PermissionId = Permission.ExitCard.UpdateId, IsActive = true },
                new RolePermission { RoleId = exitsEmployeeRole.Id, PermissionId = Permission.ExitCard.DeleteId, IsActive = true },
                new RolePermission { RoleId = exitsEmployeeRole.Id, PermissionId = Permission.ExitCard.PrintId, IsActive = true }   
            );
        }

        var birthdate = new DateOnly(2004,11,03);
        var person = Person.Create("عبدالكريم شوقي", birthdate, "782422822", null, "صنعاء", true).Value;

        if(!await _context.People.AnyAsync())
        {
            // Seed default people
            await _context.People.AddAsync(person);

            await _context.SaveChangesAsync();
        }

        // Default users
        var adminEmail = "admin@alatrafclinic.com";
        var admin = new AppUser
        {
            Id = "19a59129-6c20-417a-834d-11a208d32d96",
            UserName = "Admin",
            NormalizedUserName = "ADMIN",
            Email = adminEmail,  // ✅ ADD THIS
            NormalizedEmail = adminEmail.ToUpperInvariant(),  // ✅ ADD THIS
            EmailConfirmed = true,
            PersonId = person.Id,
            IsActive = true
        };

        if (_userManager.Users.All(u => u.UserName != admin.UserName))
        {
            // Use a proper password
            var password = "Admin@123";  // Must meet your password policy
            
            // Check if creation succeeded
            var createResult = await _userManager.CreateAsync(admin, password);
            
            if (createResult.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(adminRole.Name))
                {
                    await _userManager.AddToRolesAsync(admin, [adminRole.Name]);
                }
            }
            else
            {
                // Log the errors
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create admin user: {errors}");
            }
        }
        

        int? id1 = null;
        int? id2  = null;
        if(!await _context.Departments.AnyAsync())
        {
            var dep1 = Department.Create(1, "العلاج الطبيعي").Value;
            var dep2 = Department.Create(2, "الادارة الفنية").Value;

            _context.Departments.Add(dep1);
            _context.Departments.Add(dep2);

            await _context.SaveChangesAsync();
            id1 = dep1.Id;
            id2 = dep2.Id;

        }

        // Seed, if necessary
        if (!await _context.Services.AnyAsync())
        {
            _context.Services.AddRange(
                Service.Create(1, "استشارة", null).Value,
                Service.Create(2, "علاج طبيعي", id1).Value,
                Service.Create(3, "اطراف صناعية", id2).Value,
                Service.Create(4, "مبيعات", id2).Value,
                Service.Create(5, "إصلاحات", id2).Value,
                Service.Create(6, "عظام", id1).Value,
                Service.Create(7, "أعصاب", id1).Value,
                Service.Create(8, "تجديد كروت علاج", id1).Value,
                Service.Create(9, "إصدار بدل فاقد لكرت علاج", id1, price: 500).Value
            );

        }
        if (!await _context.InjuryTypes.AnyAsync())
        {
            _context.InjuryTypes.AddRange(
                InjuryType.Create("كسر يد").Value,
                InjuryType.Create("حرق").Value,
                InjuryType.Create("كسر قدم").Value
            );
        }
        if (!await _context.InjuryReasons.AnyAsync())
        {
            _context.InjuryReasons.AddRange(
                InjuryReason.Create("حادث").Value,
                InjuryReason.Create("إجهاد").Value,
                InjuryReason.Create("مرض").Value
            );
        }
        if (!await _context.InjurySides.AnyAsync())
        {
            _context.InjurySides.AddRange(
                InjurySide.Create("اليد المينى").Value,
                InjurySide.Create("اليد اليسرى").Value,
                InjurySide.Create("الرجل اليمنى").Value,
                InjurySide.Create("الرجل اليسرى").Value
            );
        }
        if (!await _context.TherapyCardTypePrices.AnyAsync())
        {
            _context.TherapyCardTypePrices.AddRange(
                TherapyCardTypePrice.Create(TherapyCardType.General, 200m).Value,
                TherapyCardTypePrice.Create(TherapyCardType.Special, 2000m).Value,
                TherapyCardTypePrice.Create(TherapyCardType.NerveKids, 400m).Value
            );
        }

        if (!await _context.Units.AnyAsync())
        {
            _context.Units.AddRange(
                GeneralUnit.Create("قطعة").Value,
                GeneralUnit.Create("زوج").Value,
                GeneralUnit.Create("طرف علوي يمين").Value,
                GeneralUnit.Create("طرف علوي يسار").Value,
                GeneralUnit.Create("طرف سفلي يمين").Value,
                GeneralUnit.Create("طرف سفلي يسار").Value,
                GeneralUnit.Create("الطرفين العلويين").Value,
                GeneralUnit.Create("الطرفين السفليين").Value
            );
        }
        
        if (!await _context.MedicalPrograms.AnyAsync())
        {
            _context.MedicalPrograms.AddRange(
                MedicalProgram.Create("تمارين").Value,
                MedicalProgram.Create("مساج").Value,
                MedicalProgram.Create("حرارة").Value
            );
        }

        if (!await _context.IndustrialParts.AnyAsync())
        {
            _context.IndustrialParts.AddRange(
                IndustrialPart.Create("طرف صناعي").Value,
                IndustrialPart.Create("رجل صناعي").Value,
                IndustrialPart.Create("دعامة ركبة").Value
            );
        }
        if (!await _context.AppSettings.AnyAsync())
        {
            _context.AppSettings.AddRange(
                AppSetting.Create(AlatrafClinicConstants.AllowedDaysKey, "Saturday, Tuesday").Value,
                AppSetting.Create(AlatrafClinicConstants.AppointmentDailyCapacityKey, AlatrafClinicConstants.DefaultAppointmentDailyCapacity.ToString()).Value,
                AppSetting.Create(AlatrafClinicConstants.WoundedReportMinTotalKey, "30000").Value
            );
        }

        

        // In your DbContext seed method
        if (!await _context.ReportDomains.AnyAsync())
        {
            var now = new DateTime(2026,01,07);
            
            var reportDomain = new ReportDomain
            {
                Name = "تقرير المرضى",
                RootTable = "Patients",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };
            _context.ReportDomains.Add(reportDomain);
            await _context.SaveChangesAsync();

            var reportFields = new List<ReportField>
            {
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_id",
                    DisplayName = "رقم المريض",
                    TableName = "Patients",
                    ColumnName = "PatientId",
                    DataType = "int",
                    IsFilterable = true,
                    IsSortable = true,
                    IsActive = true,
                    DisplayOrder = 1,
                    DefaultOrder = 1,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_name",
                    DisplayName = "اسم المريض",
                    TableName = "People",
                    ColumnName = "FullName",
                    DataType = "nvarchar(100)",
                    IsFilterable = true,
                    IsSortable = true,
                    IsActive = true,
                    DisplayOrder = 2,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                // Add more fields as needed
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_age",
                    DisplayName = "العمر",
                    TableName = "People",
                    ColumnName = "Age",
                    DataType = "int",
                    IsFilterable = true,
                    IsSortable = true,
                    IsActive = true,
                    DisplayOrder = 3,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_phone",
                    DisplayName = "هاتف المريض",
                    TableName = "People",
                    ColumnName = "Phone",
                    DataType = "nvarchar(15)",
                    IsFilterable = false,
                    IsSortable = false,
                    IsActive = true,
                    DisplayOrder = 4,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_type",
                    DisplayName = "نوع المريض",
                    TableName = "Patients",
                    ColumnName = "PatientType",
                    DataType = "nvarchar(50)",
                    IsFilterable = true,
                    IsSortable = true,
                    IsActive = true,
                    DisplayOrder = 5,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_address",
                    DisplayName = "العنوان",
                    TableName = "People",
                    ColumnName = "Address",
                    DataType = "nvarchar(100)",
                    IsFilterable = true,
                    IsSortable = true,
                    IsActive = true,
                    DisplayOrder = 6,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_national_number",
                    DisplayName = "رقم الهوية الوطنية",
                    TableName = "People",
                    ColumnName = "NationalNo",
                    DataType = "nvarchar(20)",
                    IsFilterable = true,
                    IsSortable = true,
                    IsActive = true,
                    DisplayOrder = 7,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_gender",
                    DisplayName = "الجنس",
                    TableName = "People",
                    ColumnName = "Gender",
                    DataType = "bit",
                    IsFilterable = true,
                    IsSortable = true,
                    IsActive = true,
                    DisplayOrder = 8,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "created_at_utc",
                    DisplayName = "تاريخ الإنشاء",
                    TableName = "Patients",
                    ColumnName = "CreatedAtUtc",
                    DataType = "datetimeoffset(7)",
                    IsFilterable = true,
                    IsSortable = true,
                    IsActive = true,
                    DisplayOrder = 9,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_ticket_id",
                    DisplayName = "رقم التذكرة",
                    TableName = "Tickets",
                    ColumnName = "TicketId",
                    DataType = "int",
                    IsFilterable = true,
                    IsSortable = true,
                    IsActive = true,
                    DisplayOrder = 10,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_service_type",
                    DisplayName = "نوع الخدمة",
                    TableName = "Services",
                    ColumnName = "Name",
                    DataType = "nvarchar(200)",
                    IsFilterable = true,
                    IsSortable = true,
                    IsActive = true,
                    DisplayOrder = 11,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_department",
                    DisplayName = "القسم",
                    TableName = "Departments",
                    ColumnName = "Name",
                    DataType = "nvarchar(100)",
                    IsFilterable = true,
                    IsSortable = true,
                    IsActive = true,
                    DisplayOrder = 12,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_diagnosis_id",
                    DisplayName = "رقم التشخيص",
                    TableName = "Diagnoses",
                    ColumnName = "DiagnosisId",
                    DataType = "int",
                    IsFilterable = false,
                    IsSortable = true,
                    IsActive = true,
                    DisplayOrder = 13,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_diagnosis_create",
                    DisplayName = "تاريخ التشخيص",
                    TableName = "Diagnoses",
                    ColumnName = "CreatedAtUtc",
                    DataType = "datetimeoffset(7)",
                    IsFilterable = true,
                    IsSortable = true,
                    IsActive = true,
                    DisplayOrder = 14,
                    CreatedAt = now,
                    UpdatedAt = now
                },
            };

            _context.ReportFields.AddRange(reportFields);

            var reportJoins = new List<ReportJoin>
            {
                new ReportJoin
                {
                    DomainId = reportDomain.Id,
                    FromTable = "Patients",
                    ToTable = "People",
                    JoinType = "INNER",
                    JoinCondition = "Patients.PersonId = People.PersonId",
                    IsActive = true,
                    IsRequired = true,
                    JoinOrder = 1,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportJoin
                {
                    DomainId = reportDomain.Id,
                    FromTable = "Patients",
                    ToTable = "Tickets",
                    JoinType = "LEFT",
                    JoinCondition = "Patients.PatientId = Tickets.PatientId",
                    IsActive = true,
                    JoinOrder = 1,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportJoin
                {
                    DomainId = reportDomain.Id,
                    FromTable = "Tickets",
                    ToTable = "Services",
                    JoinType = "LEFT",
                    JoinCondition = "Tickets.ServiceId = Services.ServiceId",
                    IsActive = true,
                    IsRequired = true,
                    JoinOrder = 2,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportJoin
                {
                    DomainId = reportDomain.Id,
                    FromTable = "Services",
                    ToTable = "Departments",
                    JoinType = "LEFT",
                    JoinCondition = "Services.DepartmentId = Departments.DepartmentId",
                    IsActive = true,
                    IsRequired = true,
                    JoinOrder = 3,
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new ReportJoin
                {
                    DomainId = reportDomain.Id,
                    FromTable = "Tickets",
                    ToTable = "Diagnoses",
                    JoinType = "LEFT",
                    JoinCondition = "Tickets.TicketId = Diagnoses.TicketId",
                    IsActive = true,
                    IsRequired = true,
                    JoinOrder = 2,
                    CreatedAt = now,
                    UpdatedAt = now
                },
            };
            _context.ReportJoins.AddRange(reportJoins);
        }
        await _context.SaveChangesAsync();
    }
}

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<AlatrafClinicDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}
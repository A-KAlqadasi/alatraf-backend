using AlatrafClinic.Application.Common.Interfaces.Repositories.Inventory;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories
{
    // Coordinates multiple repositories that share the same database context.
    // Ensures save and  commits all  tracked changes as a single  transaction. 
    public interface IUnitOfWork : IAsyncDisposable
    {
        IPersonRepository Person { get; }
        IEmployeeRepository Employees { get; }
        IPatientRepository Patients { get; }
        IDoctorRepository Doctors { get; }
        IDiagnosisRepository Diagnoses { get; }
        ITicketRepository Tickets { get; }
        IInjuryReasonRepository InjuryReasons { get; }
        IInjurySideRepository InjurySides { get; }
        IInjuryTypeRepository InjuryTypes { get; }
        IServiceRepository Services { get; }
        IDepartmentRepository Departments { get; }
        ISectionRepository Sections { get; }
        IRoomRepository Rooms { get; }
        IDoctorSectionRoomRepository DoctorSectionRooms { get; }
        IMedicalProgramRepository MedicalPrograms { get; }
        IIndustrialPartRepository IndustrialParts { get; }
        IItemRepository Items { get; }
        ISupplierRepository Suppliers { get; }
        IUnitRepository Units { get; }
        ISaleRepository Sales { get; }
        ITherapyCardRepository TherapyCards { get; }
        ITherapyCardTypePriceRepository TherapyCardTypePrices { get; }
        ISessionRepository Sessions { get; }
        IHolidayRepository Holidays { get; }
        IRepairCardRepository RepairCards { get; }
        IPaymentRepository Payments { get; }
        IAppSettingRepository AppSettings { get; }
        IAppointmentRepository Appointments { get; }
        
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
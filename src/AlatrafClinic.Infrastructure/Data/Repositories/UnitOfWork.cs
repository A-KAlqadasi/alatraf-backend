using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Interfaces.Repositories.Inventory;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public IPersonRepository Person => throw new NotImplementedException();

    public IEmployeeRepository Employees => throw new NotImplementedException();

    public IPatientRepository Patients => throw new NotImplementedException();

    public IDoctorRepository Doctors => throw new NotImplementedException();

    private IDiagnosisRepository? _diagnosis ;

    public IDiagnosisRepository Diagnoses => _diagnosis ??= new DiagnosisRepository(_dbContext); 

    public ITicketRepository Tickets => throw new NotImplementedException();

    public IInjuryReasonRepository InjuryReasons => throw new NotImplementedException();

    public IInjurySideRepository InjurySides => throw new NotImplementedException();

    public IInjuryTypeRepository InjuryTypes => throw new NotImplementedException();

    public IServiceRepository Services => throw new NotImplementedException();

    public IDepartmentRepository Departments => throw new NotImplementedException();

    public ISectionRepository Sections => throw new NotImplementedException();

    public IRoomRepository Rooms => throw new NotImplementedException();

    public IDoctorSectionRoomRepository DoctorSectionRooms => throw new NotImplementedException();

    public IMedicalProgramRepository MedicalPrograms => throw new NotImplementedException();

    public IIndustrialPartRepository IndustrialParts => throw new NotImplementedException();

    public IItemRepository Items => throw new NotImplementedException();

    public ISupplierRepository Suppliers => throw new NotImplementedException();

    public IUnitRepository Units => throw new NotImplementedException();

    public ISaleRepository Sales => throw new NotImplementedException();

    public ITherapyCardRepository TherapyCards => throw new NotImplementedException();

    public ITherapyCardTypePriceRepository TherapyCardTypePrices => throw new NotImplementedException();

    public ISessionRepository Sessions => throw new NotImplementedException();

    public IHolidayRepository Holidays => throw new NotImplementedException();

    public IRepairCardRepository RepairCards => throw new NotImplementedException();

    public IPaymentRepository Payments => throw new NotImplementedException();

    public IAppSettingRepository AppSettings => throw new NotImplementedException();

    private IAppointmentRepository? _appointments;
    public IAppointmentRepository Appointments => _appointments ??= new AppointmentRepository(_dbContext);

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _dbContext.SaveChangesAsync(ct);
    }
}
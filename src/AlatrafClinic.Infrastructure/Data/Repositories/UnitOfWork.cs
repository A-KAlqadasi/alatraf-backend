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

    private ITicketRepository? _ticket;

    public ITicketRepository Tickets => _ticket ??= new TicketRepository(_dbContext);

    public IInjuryReasonRepository InjuryReasons => throw new NotImplementedException();

    public IInjurySideRepository InjurySides => throw new NotImplementedException();

    public IInjuryTypeRepository InjuryTypes => throw new NotImplementedException();

    private IServiceRepository? _service;

    public IServiceRepository Services => _service ??= new ServiceRepository(_dbContext);

    public IDepartmentRepository Departments => throw new NotImplementedException();

    public ISectionRepository Sections => throw new NotImplementedException();

    public IRoomRepository Rooms => throw new NotImplementedException();

    public IDoctorSectionRoomRepository DoctorSectionRooms => throw new NotImplementedException();

    public IMedicalProgramRepository MedicalPrograms => throw new NotImplementedException();

    public IIndustrialPartRepository IndustrialParts => throw new NotImplementedException();

    public IItemRepository Items => throw new NotImplementedException();

    public ISupplierRepository Suppliers => throw new NotImplementedException();

    public IUnitRepository Units => throw new NotImplementedException();

    private ISaleRepository? _sale;

    public ISaleRepository Sales => _sale ??= new SaleRepository(_dbContext);

    private ITherapyCardRepository? _therapyCard;

    public ITherapyCardRepository TherapyCards => _therapyCard ??= new TherapyCardRepository(_dbContext);

    public ITherapyCardTypePriceRepository TherapyCardTypePrices => throw new NotImplementedException();

    private ISessionRepository? _session;

    public ISessionRepository Sessions => _session ??= new SessionRepository(_dbContext);

    private IHolidayRepository? _holiday;

    public IHolidayRepository Holidays => _holiday ??= new HolidayRepository(_dbContext);

    private IRepairCardRepository? _repairCard;

    public IRepairCardRepository RepairCards => _repairCard ??= new RepairCardRepository(_dbContext);

    private IPaymentRepository? _payment;

    public IPaymentRepository Payments => _payment ??= new PaymentRepository(_dbContext);

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
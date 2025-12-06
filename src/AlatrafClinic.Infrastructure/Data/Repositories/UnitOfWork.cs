using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Interfaces.Repositories.Inventory;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class UnitOfWork(AlatrafClinicDbContext dbContext) : IUnitOfWork
{
    private readonly AlatrafClinicDbContext _dbContext = dbContext;

    private IPersonRepository? _person;
    public IPersonRepository People => _person ??= new PersonRepository(_dbContext);

    private IPatientRepository? _patient;
    public IPatientRepository Patients => _patient ??= new PatientRepository(_dbContext);

    private IDoctorRepository? _doctor;
    public IDoctorRepository Doctors => _doctor ??= new DoctorRepository(_dbContext);

    private IDiagnosisRepository? _diagnosis ;
    public IDiagnosisRepository Diagnoses => _diagnosis ??= new DiagnosisRepository(_dbContext);

    private ITicketRepository? _ticket;
    public ITicketRepository Tickets => _ticket ??= new TicketRepository(_dbContext);

    private IInjuryReasonRepository? _injuryReason;
    public IInjuryReasonRepository InjuryReasons => _injuryReason ??= new InjuryReasonRepository(_dbContext);

    private IInjurySideRepository? _injurySide;
    public IInjurySideRepository InjurySides => _injurySide ??= new InjurySideRepository(_dbContext);

    private IInjuryTypeRepository? _injuryType;
    public IInjuryTypeRepository InjuryTypes => _injuryType ??= new InjuryTypeRepository(_dbContext);

    private IServiceRepository? _service;
    public IServiceRepository Services => _service ??= new ServiceRepository(_dbContext);

    private IDepartmentRepository? _department;
    public IDepartmentRepository Departments => _department ??= new DepartmentRepository(_dbContext);

    private ISectionRepository? _section;
    public ISectionRepository Sections => _section ??= new SectionRepository(_dbContext);

    private IRoomRepository? _room;
    public IRoomRepository Rooms => _room ??= new RoomRepository(_dbContext);

    private IDoctorSectionRoomRepository? _doctorSectionRoom;
    public IDoctorSectionRoomRepository DoctorSectionRooms => _doctorSectionRoom ??= new DoctorSectionRoomRepository(_dbContext);

    private IMedicalProgramRepository? _medicalProgram;
    public IMedicalProgramRepository MedicalPrograms => _medicalProgram ??= new MedicalProgramRepository(_dbContext);

    private IIndustrialPartRepository? _industrialPart;
    public IIndustrialPartRepository IndustrialParts => _industrialPart ??= new IndustrialPartRepository(_dbContext);

    
    private IItemRepository? _item;
    public IItemRepository Items => _item ??= new Inventory.ItemRepository(_dbContext);

    private ISupplierRepository? _supplier;
    public ISupplierRepository Suppliers => _supplier ??= new Inventory.SupplierRepository(_dbContext);

    private IUnitRepository? _unit;
    public IUnitRepository Units => _unit ??= new Inventory.UnitRepository(_dbContext);

    private ISaleRepository? _sale;
    public ISaleRepository Sales => _sale ??= new SaleRepository(_dbContext);

    private ITherapyCardRepository? _therapyCard;

    public ITherapyCardRepository TherapyCards => _therapyCard ??= new TherapyCardRepository(_dbContext);

    private ITherapyCardTypePriceRepository? _therapyCardTypePrices;
    public ITherapyCardTypePriceRepository TherapyCardTypePrices => _therapyCardTypePrices ??= new TherapyCardTypePriceRepository(_dbContext);

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

    private IDisabledCardRepository? _disabledCards;
    public IDisabledCardRepository DisabledCards => _disabledCards ??= new DisabledCardRepository(_dbContext);

    private IWoundedCardRepository? _woundedCards;
    public IWoundedCardRepository WoundedCards => _woundedCards ??= new WoundedCardRepository(_dbContext);

    private IStoreRepository? _store;
    public IStoreRepository Stores => _store ??= new Inventory.StoreRepository(_dbContext);

    private IExchangeOrderRepository? _exchangeOrder;
    public IExchangeOrderRepository ExchangeOrders => _exchangeOrder ??= new Inventory.ExchangeOrderRepository(_dbContext);

    private IPurchaseInvoiceRepository? _purchaseInvoice;
    public IPurchaseInvoiceRepository PurchaseInvoices => _purchaseInvoice ??= new Inventory.PurchaseInvoiceRepository(_dbContext);

    private IOrderRepository? _order;
    public IOrderRepository Orders => _order ??= new OrderRepository(_dbContext);

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _dbContext.SaveChangesAsync(ct);
    }
}
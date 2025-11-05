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
        IDiagnosisRepository Diagnosises { get; }
        ITicketRepository Tickets { get; }
        IInjuryReasonRepository InjuryReasons { get; }
        IInjurySideRepository InjurySides { get; }
        IInjuryTypeRepository InjuryTypes { get; }
        IServiceRepository Services { get; }
        IDepartmentRepository Departments { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
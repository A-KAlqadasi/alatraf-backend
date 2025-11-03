namespace AlatrafClinic.Application.Common.Interfaces.Repositories
{
    // Coordinates multiple repositories that share the same database context.
    // Ensures save and  commits all  tracked changes as a single  transaction. 
    public interface IUnitWork : IAsyncDisposable
    {
        IPersonRepository People { get; }
        IEmployeeRepository Employees { get; }
        IPatientRepository Patients { get; }
        IDoctorRepository Doctors { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
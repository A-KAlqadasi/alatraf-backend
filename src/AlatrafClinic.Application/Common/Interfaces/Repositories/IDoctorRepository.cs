using AlatrafClinic.Domain.Organization.Doctors;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IDoctorRepository : IGenericRepository<Doctor, int>
{
  Task<Doctor?> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default);
  Task<IReadOnlyList<Doctor>> GetByDepartmentIdAsync(int departmentId, CancellationToken cancellationToken = default);
  Task<IReadOnlyList<Doctor>> GetWithActiveAssignmentsAsync(CancellationToken cancellationToken = default);

    Task<IQueryable<Doctor>> GetDoctorsQueryAsync(CancellationToken ct = default);   

}

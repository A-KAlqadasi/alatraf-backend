using AlatrafClinic.Domain.RepairCards.IndustrialParts;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IIndustrialPartRepository : IGenericRepository<IndustrialPart, int>
{
    Task<IndustrialPartUnit?> GetByIdAndUnitId(int Id, int unitId, CancellationToken ct);
    Task<bool> IsExistsByName(string name, CancellationToken ct);
}
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IIndustrialPartRepository : IGenericRepository<IndustrialPart, int>
{
    Task<IndustrialPartUnit?> GetByIdAndUnit(int Id, int unitId, CancellationToken ct);
    
}
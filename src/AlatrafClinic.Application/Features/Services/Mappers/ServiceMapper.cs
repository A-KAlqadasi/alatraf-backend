using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Domain.Services;

namespace AlatrafClinic.Application.Features.Services.Mappers;

public static class ServiceMapper
{
    public static ServiceDto ToDto(this Service service)
    {
        return new ServiceDto
        {
            ServiceId = service.Id,
            Name = service.Name,
            DepartmentId = service.DepartmentId ?? 0,
            Department = service.Department?.Name ?? ""
        };
    }
    
    public static List<ServiceDto> ToDtos(this IEnumerable<Service> services)
    {
        return services.Select(s => s.ToDto()).ToList();
    }
}
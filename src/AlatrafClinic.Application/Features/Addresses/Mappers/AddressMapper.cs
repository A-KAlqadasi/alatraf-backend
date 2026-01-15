using AlatrafClinic.Application.Features.Addresses.Dtos;
using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Application.Features.Addresses.Mappers;

public static class AddressMapper
{
    public static AddressDto ToDto(this Address address) =>
        new AddressDto
        {
            Id = address.Id,
            Name = address.Name
        };
}
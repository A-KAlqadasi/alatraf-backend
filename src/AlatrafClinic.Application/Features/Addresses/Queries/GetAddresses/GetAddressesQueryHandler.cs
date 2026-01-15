using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Addresses.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Addresses.Queries.GetAddresses;

public class GetAddressesQueryHandler(
    IAppDbContext context
    ) : IRequestHandler<GetAddressesQuery, Result<PaginatedList<AddressDto>>>
{
    public async Task<Result<PaginatedList<AddressDto>>> Handle(
        GetAddressesQuery request,
        CancellationToken cancellationToken
        )
    {
        var query = context.Addresses.AsNoTracking();

        query = ApplySearch(query, request);

        var totalCount = await query.CountAsync(cancellationToken);

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        var skip = (pageNumber - 1) * pageSize;

        var addresses = await query
            .OrderBy(a => a.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(a => new AddressDto
            {
                Id = a.Id,
                Name = a.Name
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<AddressDto>
        {
            Items = addresses,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
     private static IQueryable<Address> ApplySearch(
        IQueryable<Address> query,
        GetAddressesQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.Search))
            return query;

        var pattern = $"%{q.Search!.Trim().ToLower()}%";

        return query.Where(rc =>
            EF.Functions.Like(rc.Name.ToLower(), pattern)
        );
    }
}
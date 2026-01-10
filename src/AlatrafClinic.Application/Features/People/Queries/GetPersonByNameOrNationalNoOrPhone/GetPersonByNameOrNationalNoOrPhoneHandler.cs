using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.People.Dtos;
using AlatrafClinic.Application.Features.People.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.People.Queries.GetPersonByNameOrNationalNoOrPhone;

public class GetPersonByNameOrNationalNoOrPhoneHandler
    : IRequestHandler<GetPersonByNameOrNationalNoOrPhoneQuery, Result<PersonDto>>
{
    private readonly IAppDbContext _context;

    public GetPersonByNameOrNationalNoOrPhoneHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PersonDto>> Handle(
        GetPersonByNameOrNationalNoOrPhoneQuery query,
        CancellationToken ct)
    {
        
        IQueryable<Person> personsQuery = _context.People
            .AsNoTracking();
        
        personsQuery = ApplyFilters(personsQuery, query);

        var person = await personsQuery.FirstOrDefaultAsync(ct);
        
        if (person is null)
        {
            return PersonErrors.PersonNotFound;
        }

        return person.ToDto();
    }

    private static IQueryable<Person> ApplyFilters(
        IQueryable<Person> query,
        GetPersonByNameOrNationalNoOrPhoneQuery q)
    {
        
        if (q.Name is not null)
        {
            var name = q.Name.Trim().ToLower();
            query = query.Where(p =>
                p.FullName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        if (q.NationalNo is not null)
        {
            var nationalNo = q.NationalNo.Trim();
            query = query.Where(p =>
                p.NationalNo == nationalNo);
        }

        if (q.Phone is not null)
        {
            var phone =  q.Phone.Trim();
            query = query.Where(p =>
                p.Phone == phone);
        }
        
        return query;
    }
}
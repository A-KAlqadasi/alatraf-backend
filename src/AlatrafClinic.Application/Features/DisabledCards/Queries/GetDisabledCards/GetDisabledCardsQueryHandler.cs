using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.DisabledCards.Dtos;
using AlatrafClinic.Application.Features.DisabledCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.DisabledCards;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.DisabledCards.Queries.GetDisabledCards;

public sealed class GetDisabledCardsQueryHandler
    : IRequestHandler<GetDisabledCardsQuery, Result<PaginatedList<DisabledCardDto>>>
{
    private readonly IAppDbContext _context;

    public GetDisabledCardsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<DisabledCardDto>>> Handle(
        GetDisabledCardsQuery query,
        CancellationToken ct)
    {
        // Base query + includes
        IQueryable<DisabledCard> cardsQuery = _context.DisabledCards
            .Include(dc => dc.Patient)
                .ThenInclude(p => p.Person)
            .AsNoTracking();

        // Apply filters, search, sorting
        cardsQuery = ApplyFilters(cardsQuery, query);
        cardsQuery = ApplySearch(cardsQuery, query);
        cardsQuery = ApplySorting(cardsQuery, query);

        var totalCount = await cardsQuery.CountAsync(ct);

        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        // Paging
        var cards = await cardsQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        // Mapping – same as before (using your extension)
        var items = cards
            .Select(dc => dc.ToDto())
            .ToList();

        return new PaginatedList<DisabledCardDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    // ---------------- FILTERS ----------------
    private static IQueryable<DisabledCard> ApplyFilters(
        IQueryable<DisabledCard> query,
        GetDisabledCardsQuery q)
    {
        // filter by "expired" using ExpirationDate vs today
        if (q.IsExpired.HasValue)
        {
            var today = DateTime.Today;
            if (q.IsExpired.Value)
            {
                query = query.Where(dc => dc.ExpirationDate < today);
            }
            else
            {
                query = query.Where(dc => dc.ExpirationDate >= today);
            }
        }

        if (q.PatientId.HasValue && q.PatientId.Value > 0)
        {
            var patientId = q.PatientId.Value;
            query = query.Where(dc => dc.PatientId == patientId);
        }

        if (q.ExpirationFrom.HasValue)
        {
            var from = q.ExpirationFrom.Value.Date;
            query = query.Where(dc => dc.ExpirationDate >= from);
        }

        if (q.ExpirationTo.HasValue)
        {
            var to = q.ExpirationTo.Value.Date;
            query = query.Where(dc => dc.ExpirationDate <= to);
        }

        return query;
    }

    // ---------------- SEARCH ----------------
    private static IQueryable<DisabledCard> ApplySearch(
        IQueryable<DisabledCard> query,
        GetDisabledCardsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(dc =>
            EF.Functions.Like(dc.CardNumber.ToLower(), pattern) ||
            (dc.Patient != null &&
             dc.Patient.Person != null &&
             EF.Functions.Like(dc.Patient.Person.FullName.ToLower(), pattern)));
    }

    // ---------------- SORTING ----------------
    private static IQueryable<DisabledCard> ApplySorting(
        IQueryable<DisabledCard> query,
        GetDisabledCardsQuery q)
    {
        var col = q.SortColumn?.Trim().ToLowerInvariant() ?? "expirationdate";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "cardnumber" => isDesc
                ? query.OrderByDescending(dc => dc.CardNumber)
                : query.OrderBy(dc => dc.CardNumber),

            "patient" => isDesc
                ? query.OrderByDescending(dc => dc.Patient!.Person!.FullName)
                : query.OrderBy(dc => dc.Patient!.Person!.FullName),

            "expirationdate" => isDesc
                ? query.OrderByDescending(dc => dc.ExpirationDate)
                : query.OrderBy(dc => dc.ExpirationDate),

            _ => isDesc
                ? query.OrderByDescending(dc => dc.ExpirationDate)
                : query.OrderBy(dc => dc.ExpirationDate),
        };
    }
}

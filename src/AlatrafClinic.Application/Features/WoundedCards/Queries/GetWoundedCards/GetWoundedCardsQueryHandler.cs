using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.WoundedCards.Dtos;
using AlatrafClinic.Application.Features.WoundedCards.Mappers;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.WoundedCards;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.WoundedCards.Queries.GetWoundedCards;

public class GetWoundedCardsQueryHandler
    : IRequestHandler<GetWoundedCardsQuery, Result<PaginatedList<WoundedCardDto>>>
{
    private readonly IAppDbContext _context;

    public GetWoundedCardsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<WoundedCardDto>>> Handle(
        GetWoundedCardsQuery query,
        CancellationToken ct)
    {
        IQueryable<WoundedCard> cardsQuery = _context.WoundedCards
            .Include(wc => wc.Patient)
                .ThenInclude(p => p.Person)
            .AsNoTracking();

        cardsQuery = ApplyFilters(cardsQuery, query);
        cardsQuery = ApplySearch(cardsQuery, query);
        cardsQuery = ApplySorting(cardsQuery, query);

        var totalCount = await cardsQuery.CountAsync(ct);

        var page = query.Page;
        var pageSize = query.PageSize;
        var skip = (page - 1) * pageSize;

        var cards = await cardsQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = cards
            .ToDtos();

        return new PaginatedList<WoundedCardDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static IQueryable<WoundedCard> ApplyFilters(
        IQueryable<WoundedCard> query,
        GetWoundedCardsQuery q)
    {
        var today = AlatrafClinicConstants.TodayDate;

        if (q.IsExpired.HasValue)
        {
            if (q.IsExpired.Value)
            {
                query = query.Where(wc => wc.ExpirationDate < today);
            }
            else
            {
                query = query.Where(wc => wc.ExpirationDate >= today);
            }
        }

        if (q.PatientId.HasValue && q.PatientId.Value > 0)
        {
            var patientId = q.PatientId.Value;
            query = query.Where(wc => wc.PatientId == patientId);
        }

        if (q.ExpirationFrom.HasValue)
        {
            var from = q.ExpirationFrom.Value;
            query = query.Where(wc => wc.ExpirationDate >= from);
        }

        if (q.ExpirationTo.HasValue)
        {
            var to = q.ExpirationTo.Value;
            query = query.Where(wc => wc.ExpirationDate <= to);
        }

         if (q.IssueDateFrom.HasValue)
        {
            var from = q.IssueDateFrom.Value;
            query = query.Where(wc => wc.IssueDate >= from);
        }

        if (q.IssueDateTo.HasValue)
        {
            var to = q.IssueDateTo.Value;
            query = query.Where(wc => wc.IssueDate <= to);
        }

        return query;
    }

    private static IQueryable<WoundedCard> ApplySearch(
        IQueryable<WoundedCard> query,
        GetWoundedCardsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(wc =>
            EF.Functions.Like(wc.CardNumber.ToLower(), pattern) ||
            (wc.Patient != null &&
             wc.Patient.Person != null &&
             EF.Functions.Like(wc.Patient.Person.FullName.ToLower(), pattern)));
    }

    private static IQueryable<WoundedCard> ApplySorting(
        IQueryable<WoundedCard> query,
        GetWoundedCardsQuery q)
    {
        var col = q.SortColumn?.Trim().ToLowerInvariant() ?? "expiration";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "cardnumber" => isDesc
                ? query.OrderByDescending(wc => wc.CardNumber)
                : query.OrderBy(wc => wc.CardNumber),

            "patient" => isDesc
                ? query.OrderByDescending(wc => wc.Patient!.Person!.FullName)
                : query.OrderBy(wc => wc.Patient!.Person!.FullName),

            "expiration" => isDesc
                ? query.OrderByDescending(wc => wc.ExpirationDate)
                : query.OrderBy(wc => wc.ExpirationDate),

            _ => isDesc
                ? query.OrderByDescending(wc => wc.ExpirationDate)
                : query.OrderBy(wc => wc.ExpirationDate),
        };
    }
}

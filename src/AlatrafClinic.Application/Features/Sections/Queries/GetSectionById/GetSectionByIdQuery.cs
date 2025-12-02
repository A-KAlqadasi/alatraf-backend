using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sections.Queries.GetSectionById;

public sealed record class GetSectionByIdQuery(int SectionId) : IRequest<Result<SectionDto>>;
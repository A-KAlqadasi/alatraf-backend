using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Application.Features.Organization.Sections.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Organization.Sections.Commands.CreateSection;

public sealed record CreateSectionCommand(
    int DepartmentId,
    List<string> SectionNames
) : IRequest<Result<List<SectionDto>>>;

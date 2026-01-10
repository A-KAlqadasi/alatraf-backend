using AlatrafClinic.Application.Reports.Dtos;

using FluentValidation;

namespace AlatrafClinic.Application.Reports.Validators;

public class ReportRequestValidator : AbstractValidator<ReportRequestDto>
{
    private static readonly HashSet<string> _validSortDirections = new() { "ASC", "DESC" };

    public ReportRequestValidator()
    {
        RuleFor(x => x.DomainId)
            .GreaterThan(0)
            .WithMessage("Domain ID must be greater than 0");

        RuleFor(x => x.SelectedFields)
            .NotEmpty()
            .WithMessage("At least one field must be selected")
            .Must(fields => fields.Count <= 50)
            .WithMessage("Maximum 50 fields can be selected");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.MaxRows)
            .InclusiveBetween(1, 250000)
            .WithMessage("Max rows must be between 1 and 250,000");

        RuleFor(x => x.PageSize)
            .LessThanOrEqualTo(x => x.MaxRows)
            .WithMessage("Page size cannot exceed maximum rows");

        RuleForEach(x => x.Filters)
            .SetValidator(new ReportFilterValidator());

        RuleForEach(x => x.SortBy)
            .SetValidator(new ReportSortValidator());
    }
}

public class ReportFilterValidator : AbstractValidator<ReportFilterDto>
{
    private static readonly HashSet<string> _validOperators = new(StringComparer.OrdinalIgnoreCase)
    {
        "=", "!=", "<>", ">", "<", ">=", "<=",
        "LIKE", "NOT LIKE", "IN", "NOT IN", "BETWEEN",
        "IS NULL", "IS NOT NULL"
    };

    public ReportFilterValidator()
    {
        RuleFor(x => x.FieldKey)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Operator)
            .NotEmpty()
            .Must(op => _validOperators.Contains(op))
            .WithMessage("Invalid operator. Valid operators are: " + string.Join(", ", _validOperators));

        When(x => !x.Operator.ToUpper().EndsWith("NULL"), () =>
        {
            RuleFor(x => x.Value)
                .NotNull()
                .WithMessage("Value is required for non-null operators");
        });
    }
}

public class ReportSortValidator : AbstractValidator<ReportSortDto>
{
    public ReportSortValidator()
    {
        RuleFor(x => x.FieldKey)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Direction)
            .NotEmpty()
            .Must(d => d.ToUpper() == "ASC" || d.ToUpper() == "DESC")
            .WithMessage("Sort direction must be ASC or DESC");
    }
}
namespace AlatrafClinic.Application.Common.Specifications;

public abstract class FilterSpecification<TEntity> : Specification<TEntity>
{
    public int Page { get; protected set; }
    public int PageSize { get; protected set; }

    protected FilterSpecification(int page, int pageSize)
    {
        Page = page < 1 ? 1 : page;
        PageSize = pageSize < 1 ? 10 : pageSize;
    }
}

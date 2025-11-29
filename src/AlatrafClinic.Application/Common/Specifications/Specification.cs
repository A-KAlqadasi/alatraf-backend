namespace AlatrafClinic.Application.Common.Specifications;

public abstract class Specification<T> : ISpecification<T>
{
    public abstract IQueryable<T> Apply(IQueryable<T> query);
}
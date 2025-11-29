namespace AlatrafClinic.Application.Common.Specifications;

public interface ISpecification<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
}

namespace AlatrafClinic.Application.Common.Printing.Interfaces;

public interface IPdfGenerator<T>
{
    byte[] Generate(T model, PrintContext context);
}
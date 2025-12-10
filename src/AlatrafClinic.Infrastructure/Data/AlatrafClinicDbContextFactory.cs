using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AlatrafClinic.Infrastructure.Data;

public class AlatrafClinicDbContextFactory
    : IDesignTimeDbContextFactory<AlatrafClinicDbContext>
{
    public AlatrafClinicDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AlatrafClinicDbContext>();

        var connectionString =
              "Server=.;Database=AlatrafClinicDevDb;User Id=sa;Password=sa123456;TrustServerCertificate=True;";

        optionsBuilder.UseSqlServer(connectionString);

        return new AlatrafClinicDbContext(optionsBuilder.Options);
    }
}

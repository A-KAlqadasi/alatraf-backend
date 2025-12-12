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
              "Server=Abu-Ghaith\\AMSSQLSERVER;Database=AlatrafClinicDevDb;User Id=sa;Password=isa123456;TrustServerCertificate=True;";

        optionsBuilder.UseSqlServer(connectionString);

        return new AlatrafClinicDbContext(optionsBuilder.Options);
    }
}

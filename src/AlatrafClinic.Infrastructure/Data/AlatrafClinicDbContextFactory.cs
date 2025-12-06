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
            "Server =Abu-Ghaith\\AMSSQLSERVER; Database = AlatrafClinicDevDb ; Trusted_Connection=True; MultipleActiveResultSets = true ; TrustServerCertificate = True;";

        optionsBuilder.UseSqlServer(connectionString);

        return new AlatrafClinicDbContext(optionsBuilder.Options);
    }
}

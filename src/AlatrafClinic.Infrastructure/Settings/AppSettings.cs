namespace AlatrafClinic.Infrastructure.Settings;

public class AppSettings
{
    public int LocalCacheExpirationInMins { get; set; }
    public int DistributedCacheExpirationMins { get; set; }
    public int DefaultPageNumber { get; set; }
    public int DefaultPageSize { get; set; }
    public string CorsPolicyName { get; set; } = default!;
    public string[] AllowedOrigins { get; set; } = default!;
}
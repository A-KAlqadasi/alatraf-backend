using AlatrafClinic.Application.Common.Interfaces;

public sealed class IdempotencyContext : IIdempotencyContext
{
    public string? Key { get; private set; }
    public string? Route { get; private set; }
    public string? RequestHash { get; private set; }

    public void Set(string key, string route, string requestHash)
    {
        Key = key;
        Route = route;
        RequestHash = requestHash;
    }
}

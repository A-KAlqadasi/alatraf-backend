public interface IIdempotencyContext
{
    string? Key { get; }
    string? Route { get; }
    string? RequestHash { get; }

    void Set(string key, string route, string requestHash);
}

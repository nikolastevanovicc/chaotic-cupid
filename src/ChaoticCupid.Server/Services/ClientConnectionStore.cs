namespace ChaoticCupid.Server.Services;

public sealed class ClientConnectionStore
{
    private readonly Lock _lock = new();
    private readonly Dictionary<string, string> _connectionIdsByUsername = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _usernamesByConnectionId = new(StringComparer.Ordinal);

    public void Register(string username, string connectionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);

        lock (_lock)
        {
            if (_connectionIdsByUsername.Remove(username, out var oldConnectionId))
            {
                _usernamesByConnectionId.Remove(oldConnectionId);
            }

            _connectionIdsByUsername[username] = connectionId;
            _usernamesByConnectionId[connectionId] = username;
        }
    }

    public bool TryGetConnectionId(string username, out string connectionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        lock (_lock)
        {
            return _connectionIdsByUsername.TryGetValue(username, out connectionId!);
        }
    }

    public void RemoveByConnectionId(string connectionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);

        lock (_lock)
        {
            if (!_usernamesByConnectionId.Remove(connectionId, out var username))
            {
                return;
            }

            _connectionIdsByUsername.Remove(username);
        }
    }
}

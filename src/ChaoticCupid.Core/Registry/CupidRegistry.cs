using ChaoticCupid.Core.Models;

namespace ChaoticCupid.Core.Registry;

public sealed class CupidRegistry
{
    private readonly Lock _lock = new();
    private readonly Dictionary<string, RegisteredPersonState> _people = new(StringComparer.OrdinalIgnoreCase);

    public bool RegisterPerson(SinglePerson person)
    {
        ArgumentNullException.ThrowIfNull(person);

        lock (_lock)
        {
            return _people.TryAdd(person.Username, new RegisteredPersonState(person));
        }
    }

    public bool BlockUser(string username, string blockedUsername)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(blockedUsername);

        lock (_lock)
        {
            if (!_people.TryGetValue(username, out var state))
            {
                return false;
            }

            state.BlockedUsernames.Add(blockedUsername);
            return true;
        }
    }

    public bool MarkLetterPending(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        lock (_lock)
        {
            if (!_people.TryGetValue(username, out var state) || state.HasPendingLetter)
            {
                return false;
            }

            state.HasPendingLetter = true;
            return true;
        }
    }

    public bool ConfirmLetterReceived(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        lock (_lock)
        {
            if (!_people.TryGetValue(username, out var state) || !state.HasPendingLetter)
            {
                return false;
            }

            state.HasPendingLetter = false;
            return true;
        }
    }

    public IReadOnlyList<SinglePerson> GetAvailableRecipients()
    {
        lock (_lock)
        {
            return _people.Values
                .Where(state => !state.HasPendingLetter)
                .Select(state => state.Person)
                .ToList();
        }
    }

    public IReadOnlyList<SinglePerson> GetCandidatesFor(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        lock (_lock)
        {
            if (!_people.TryGetValue(username, out var recipientState))
            {
                return [];
            }

            return _people.Values
                .Select(state => state.Person)
                .Where(candidate => !IsSameUser(recipientState.Person, candidate))
                .Where(candidate => !IsBlocked(recipientState, candidate))
                .ToList();
        }
    }

    public RegisteredPerson? FindByUsername(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        lock (_lock)
        {
            return _people.TryGetValue(username, out var state)
                ? state.ToSnapshot()
                : null;
        }
    }

    public IReadOnlyList<RegisteredPerson> GetAll()
    {
        lock (_lock)
        {
            return _people.Values
                .Select(state => state.ToSnapshot())
                .ToList();
        }
    }

    private static bool IsBlocked(RegisteredPersonState recipientState, SinglePerson candidate) =>
        recipientState.BlockedUsernames.Contains(candidate.Username);

    private static bool IsSameUser(SinglePerson first, SinglePerson second) =>
        string.Equals(first.Username, second.Username, StringComparison.OrdinalIgnoreCase);

    private sealed class RegisteredPersonState(SinglePerson person)
    {
        public SinglePerson Person { get; } = person;

        public bool HasPendingLetter { get; set; }

        public HashSet<string> BlockedUsernames { get; } = new(StringComparer.OrdinalIgnoreCase);

        public RegisteredPerson ToSnapshot() =>
            new(Person, HasPendingLetter, BlockedUsernames.ToHashSet(StringComparer.OrdinalIgnoreCase));
    }
}

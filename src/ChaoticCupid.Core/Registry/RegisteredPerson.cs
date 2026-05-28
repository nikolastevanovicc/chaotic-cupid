using ChaoticCupid.Core.Models;

namespace ChaoticCupid.Core.Registry;

public sealed record RegisteredPerson(
    SinglePerson Person,
    bool HasPendingLetter,
    IReadOnlySet<string> BlockedUsernames);

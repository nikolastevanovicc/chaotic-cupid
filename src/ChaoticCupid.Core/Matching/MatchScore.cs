using ChaoticCupid.Core.Models;

namespace ChaoticCupid.Core.Matching;

public sealed record MatchScore(
    SinglePerson Sender,
    int LocationPoints,
    int AgePoints,
    int RandomPoints)
{
    public int Total => LocationPoints + AgePoints + RandomPoints;
}

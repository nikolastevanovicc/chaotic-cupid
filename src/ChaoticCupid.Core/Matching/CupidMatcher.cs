using ChaoticCupid.Core.Models;

namespace ChaoticCupid.Core.Matching;

public sealed class CupidMatcher
{
    private const int SameCityPoints = 30;
    private const int SimilarAgePoints = 20;
    private const int MaxAgeDifference = 2;
    private const int MinRandomPoints = 0;
    private const int MaxRandomPoints = 100;

    private readonly IRandomScoreProvider _randomScoreProvider;

    public CupidMatcher()
        : this(new CryptoRandomScoreProvider())
    {
    }

    public CupidMatcher(IRandomScoreProvider randomScoreProvider)
    {
        ArgumentNullException.ThrowIfNull(randomScoreProvider);

        _randomScoreProvider = randomScoreProvider;
    }

    public MatchScore? FindBestMatch(
        SinglePerson recipient,
        IEnumerable<SinglePerson> candidates,
        IReadOnlySet<string>? blockedUsernames = null)
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.ThrowIfNull(candidates);

        return candidates
            .Where(candidate => IsEligible(recipient, candidate, blockedUsernames))
            .Select(candidate => CalculateScore(recipient, candidate))
            .OrderByDescending(score => score.Total)
            .ThenBy(score => score.Sender.Username, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();
    }

    public MatchScore CalculateScore(SinglePerson recipient, SinglePerson sender)
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.ThrowIfNull(sender);

        if (IsSameUser(recipient, sender))
        {
            throw new InvalidOperationException("A person cannot receive a letter from themselves.");
        }

        var locationPoints = HasSameCity(recipient, sender) ? SameCityPoints : 0;
        var agePoints = HasSimilarAge(recipient, sender) ? SimilarAgePoints : 0;
        var randomPoints = _randomScoreProvider.NextInclusive(MinRandomPoints, MaxRandomPoints);

        return new MatchScore(sender, locationPoints, agePoints, randomPoints);
    }

    public static bool IsEligible(
        SinglePerson recipient,
        SinglePerson sender,
        IReadOnlySet<string>? blockedUsernames = null)
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.ThrowIfNull(sender);

        if (IsSameUser(recipient, sender))
        {
            return false;
        }

        return blockedUsernames is null ||
            !blockedUsernames.Any(username => string.Equals(username, sender.Username, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsSameUser(SinglePerson first, SinglePerson second) =>
        string.Equals(first.Username, second.Username, StringComparison.OrdinalIgnoreCase);

    private static bool HasSameCity(SinglePerson first, SinglePerson second) =>
        string.Equals(first.City, second.City, StringComparison.OrdinalIgnoreCase);

    private static bool HasSimilarAge(SinglePerson first, SinglePerson second) =>
        Math.Abs(first.Age - second.Age) <= MaxAgeDifference;
}

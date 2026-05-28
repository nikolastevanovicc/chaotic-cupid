using ChaoticCupid.Core.Matching;
using ChaoticCupid.Core.Models;
using Xunit;

namespace ChaoticCupid.Tests.Matching;

public sealed class CupidMatcherTests
{
    [Fact]
    public void CalculateScore_AddsLocationAndAgePoints_WhenCityAndAgeMatch()
    {
        var matcher = new CupidMatcher(new FixedRandomScoreProvider(10));
        var recipient = Person("ana", "Novi Sad", 24);
        var sender = Person("marko", "Novi Sad", 26);

        var score = matcher.CalculateScore(recipient, sender);

        Assert.Equal(30, score.LocationPoints);
        Assert.Equal(20, score.AgePoints);
        Assert.Equal(10, score.RandomPoints);
        Assert.Equal(60, score.Total);
    }

    [Fact]
    public void CalculateScore_DoesNotAddLocationOrAgePoints_WhenCityAndAgeDoNotMatch()
    {
        var matcher = new CupidMatcher(new FixedRandomScoreProvider(7));
        var recipient = Person("ana", "Novi Sad", 24);
        var sender = Person("marko", "Beograd", 30);

        var score = matcher.CalculateScore(recipient, sender);

        Assert.Equal(0, score.LocationPoints);
        Assert.Equal(0, score.AgePoints);
        Assert.Equal(7, score.RandomPoints);
        Assert.Equal(7, score.Total);
    }

    [Fact]
    public void CalculateScore_Throws_WhenRecipientAndSenderAreSameUser()
    {
        var matcher = new CupidMatcher(new FixedRandomScoreProvider(0));
        var recipient = Person("ana", "Novi Sad", 24);
        var sender = Person("ANA", "Beograd", 30);

        Assert.Throws<InvalidOperationException>(() => matcher.CalculateScore(recipient, sender));
    }

    [Fact]
    public void IsEligible_ReturnsFalse_WhenSenderIsBlocked()
    {
        var recipient = Person("ana", "Novi Sad", 24);
        var sender = Person("Marko", "Novi Sad", 25);
        var blockedUsernames = new HashSet<string> { "marko" };

        var isEligible = CupidMatcher.IsEligible(recipient, sender, blockedUsernames);

        Assert.False(isEligible);
    }

    [Fact]
    public void FindBestMatch_SkipsSelfAndBlockedUsers()
    {
        var matcher = new CupidMatcher(new FixedRandomScoreProvider(10));
        var recipient = Person("ana", "Novi Sad", 24);
        var blocked = Person("marko", "Novi Sad", 24);
        var eligible = Person("jelena", "Subotica", 35);
        var candidates = new[] { recipient, blocked, eligible };
        var blockedUsernames = new HashSet<string> { "marko" };

        var match = matcher.FindBestMatch(recipient, candidates, blockedUsernames);

        Assert.NotNull(match);
        Assert.Equal("jelena", match.Sender.Username);
    }

    [Fact]
    public void FindBestMatch_ReturnsCandidateWithHighestScore()
    {
        var matcher = new CupidMatcher(new SequenceRandomScoreProvider(0, 100));
        var recipient = Person("ana", "Novi Sad", 24);
        var sameCityLowRandom = Person("marko", "Novi Sad", 24);
        var differentCityHighRandom = Person("jelena", "Beograd", 30);

        var match = matcher.FindBestMatch(
            recipient,
            new[] { sameCityLowRandom, differentCityHighRandom });

        Assert.NotNull(match);
        Assert.Equal("jelena", match.Sender.Username);
        Assert.Equal(100, match.RandomPoints);
    }

    [Fact]
    public void FindBestMatch_ReturnsNull_WhenThereAreNoEligibleCandidates()
    {
        var matcher = new CupidMatcher(new FixedRandomScoreProvider(10));
        var recipient = Person("ana", "Novi Sad", 24);
        var blocked = Person("marko", "Novi Sad", 24);
        var candidates = new[] { recipient, blocked };
        var blockedUsernames = new HashSet<string> { "marko" };

        var match = matcher.FindBestMatch(recipient, candidates, blockedUsernames);

        Assert.Null(match);
    }

    private static SinglePerson Person(string username, string city, int age) =>
        new(username, city, age, "060123456");

    private sealed class FixedRandomScoreProvider(int value) : IRandomScoreProvider
    {
        public int NextInclusive(int minValue, int maxValue) => value;
    }

    private sealed class SequenceRandomScoreProvider(params int[] values) : IRandomScoreProvider
    {
        private readonly Queue<int> _values = new(values);

        public int NextInclusive(int minValue, int maxValue) => _values.Dequeue();
    }
}

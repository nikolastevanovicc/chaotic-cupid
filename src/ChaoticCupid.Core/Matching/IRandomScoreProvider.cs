namespace ChaoticCupid.Core.Matching;

public interface IRandomScoreProvider
{
    int NextInclusive(int minValue, int maxValue);
}

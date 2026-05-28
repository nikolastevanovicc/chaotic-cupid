using System.Security.Cryptography;

namespace ChaoticCupid.Core.Matching;

public sealed class CryptoRandomScoreProvider : IRandomScoreProvider
{
    public int NextInclusive(int minValue, int maxValue)
    {
        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(minValue), "Minimum value must be lower than maximum value.");
        }

#pragma warning disable SYSLIB0023
        using var rng = new RNGCryptoServiceProvider();
#pragma warning restore SYSLIB0023

        return NextInclusive(rng, minValue, maxValue);
    }

    private static int NextInclusive(RandomNumberGenerator rng, int minValue, int maxValue)
    {
        var range = (uint)(maxValue - minValue + 1);
        var limit = uint.MaxValue - (uint.MaxValue % range);
        Span<byte> buffer = stackalloc byte[4];

        uint value;
        do
        {
            rng.GetBytes(buffer);
            value = BitConverter.ToUInt32(buffer);
        }
        while (value >= limit);

        return minValue + (int)(value % range);
    }
}

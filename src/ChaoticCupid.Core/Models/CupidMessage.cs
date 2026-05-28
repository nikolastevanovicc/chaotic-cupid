namespace ChaoticCupid.Core.Models;

public enum CupidMessage
{
    LookingForwardToMeet,
    WantsToMeet,
    NotInterested
}

public static class CupidMessageExtensions
{
    public static string ToDisplayText(this CupidMessage message) =>
        message switch
        {
            CupidMessage.LookingForwardToMeet => "Radujem se nasem susretu!",
            CupidMessage.WantsToMeet => "Zelim da se upoznamo.",
            CupidMessage.NotInterested => "Nisam zainteresovan/a za upoznavanje.",
            _ => throw new ArgumentOutOfRangeException(nameof(message), message, "Unknown cupid message.")
        };

    public static bool ShouldShowPhoneNumber(this CupidMessage message) =>
        message != CupidMessage.NotInterested;
}

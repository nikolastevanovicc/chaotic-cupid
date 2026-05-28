namespace ChaoticCupid.Core.Models;

public sealed record LoveLetterNotification(
    string SenderUsername,
    string SenderCity,
    int SenderAge,
    string? SenderPhoneNumber,
    string Message)
{
    public static LoveLetterNotification FromLoveLetter(LoveLetter letter)
    {
        ArgumentNullException.ThrowIfNull(letter);

        return new LoveLetterNotification(
            letter.Sender.Username,
            letter.Sender.City,
            letter.Sender.Age,
            letter.ShouldShowSenderPhoneNumber ? letter.Sender.PhoneNumber : null,
            letter.Message.ToDisplayText());
    }
}

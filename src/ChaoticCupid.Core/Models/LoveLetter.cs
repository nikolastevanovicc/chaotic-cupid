namespace ChaoticCupid.Core.Models;

public sealed record LoveLetter(
    SinglePerson Sender,
    SinglePerson Recipient,
    CupidMessage Message)
{
    public bool ShouldShowSenderPhoneNumber => Message.ShouldShowPhoneNumber();
}

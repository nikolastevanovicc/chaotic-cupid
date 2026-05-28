using ChaoticCupid.Core.Models;
using Xunit;

namespace ChaoticCupid.Tests.Models;

public sealed class LoveLetterNotificationTests
{
    [Fact]
    public void FromLoveLetter_IncludesSenderPhoneNumber_WhenMessageIsInterested()
    {
        var sender = Person("marko", "060123456");
        var recipient = Person("ana", "060000000");
        var letter = new LoveLetter(sender, recipient, CupidMessage.WantsToMeet);

        var notification = LoveLetterNotification.FromLoveLetter(letter);

        Assert.Equal("marko", notification.SenderUsername);
        Assert.Equal("Novi Sad", notification.SenderCity);
        Assert.Equal(24, notification.SenderAge);
        Assert.Equal("060123456", notification.SenderPhoneNumber);
        Assert.Equal("Zelim da se upoznamo.", notification.Message);
    }

    [Fact]
    public void FromLoveLetter_HidesSenderPhoneNumber_WhenMessageIsNotInterested()
    {
        var sender = Person("marko", "060123456");
        var recipient = Person("ana", "060000000");
        var letter = new LoveLetter(sender, recipient, CupidMessage.NotInterested);

        var notification = LoveLetterNotification.FromLoveLetter(letter);

        Assert.Null(notification.SenderPhoneNumber);
        Assert.Equal("Nisam zainteresovan/a za upoznavanje.", notification.Message);
    }

    private static SinglePerson Person(string username, string phoneNumber) =>
        new(username, "Novi Sad", 24, phoneNumber);
}

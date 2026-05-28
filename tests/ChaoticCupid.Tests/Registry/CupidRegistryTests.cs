using ChaoticCupid.Core.Models;
using ChaoticCupid.Core.Registry;
using Xunit;

namespace ChaoticCupid.Tests.Registry;

public sealed class CupidRegistryTests
{
    [Fact]
    public void RegisterPerson_AddsPerson_WhenUsernameIsNew()
    {
        var registry = new CupidRegistry();
        var person = Person("ana");

        var registered = registry.RegisterPerson(person);
        var savedPerson = registry.FindByUsername("ANA");

        Assert.True(registered);
        Assert.NotNull(savedPerson);
        Assert.Equal(person, savedPerson.Person);
        Assert.False(savedPerson.HasPendingLetter);
        Assert.Empty(savedPerson.BlockedUsernames);
    }

    [Fact]
    public void RegisterPerson_ReturnsFalse_WhenUsernameAlreadyExists()
    {
        var registry = new CupidRegistry();

        var firstRegistration = registry.RegisterPerson(Person("ana"));
        var secondRegistration = registry.RegisterPerson(Person("ANA"));

        Assert.True(firstRegistration);
        Assert.False(secondRegistration);
        Assert.Single(registry.GetAll());
    }

    [Fact]
    public void BlockUser_AddsBlockedUsername_WhenPersonExists()
    {
        var registry = new CupidRegistry();
        registry.RegisterPerson(Person("ana"));

        var blocked = registry.BlockUser("ANA", "Marko");
        var savedPerson = registry.FindByUsername("ana");

        Assert.True(blocked);
        Assert.NotNull(savedPerson);
        Assert.Contains("marko", savedPerson.BlockedUsernames, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void BlockUser_ReturnsFalse_WhenPersonDoesNotExist()
    {
        var registry = new CupidRegistry();

        var blocked = registry.BlockUser("ana", "marko");

        Assert.False(blocked);
    }

    [Fact]
    public void GetCandidatesFor_ExcludesSelfAndBlockedUsers()
    {
        var registry = new CupidRegistry();
        registry.RegisterPerson(Person("ana"));
        registry.RegisterPerson(Person("marko"));
        registry.RegisterPerson(Person("jelena"));
        registry.BlockUser("ana", "MARKO");

        var candidates = registry.GetCandidatesFor("ANA");

        Assert.DoesNotContain(candidates, candidate => candidate.Username == "ana");
        Assert.DoesNotContain(candidates, candidate => candidate.Username == "marko");
        Assert.Contains(candidates, candidate => candidate.Username == "jelena");
    }

    [Fact]
    public void GetCandidatesFor_ReturnsEmptyList_WhenPersonDoesNotExist()
    {
        var registry = new CupidRegistry();
        registry.RegisterPerson(Person("marko"));

        var candidates = registry.GetCandidatesFor("ana");

        Assert.Empty(candidates);
    }

    [Fact]
    public void MarkLetterPending_MarksPersonAsUnavailableUntilConfirmation()
    {
        var registry = new CupidRegistry();
        registry.RegisterPerson(Person("ana"));
        registry.RegisterPerson(Person("marko"));

        var marked = registry.MarkLetterPending("ANA");
        var secondMark = registry.MarkLetterPending("ana");
        var availableRecipients = registry.GetAvailableRecipients();

        Assert.True(marked);
        Assert.False(secondMark);
        Assert.DoesNotContain(availableRecipients, person => person.Username == "ana");
        Assert.Contains(availableRecipients, person => person.Username == "marko");
    }

    [Fact]
    public void ConfirmLetterReceived_ClearsPendingStatus()
    {
        var registry = new CupidRegistry();
        registry.RegisterPerson(Person("ana"));
        registry.MarkLetterPending("ana");

        var confirmed = registry.ConfirmLetterReceived("ANA");
        var secondConfirmation = registry.ConfirmLetterReceived("ana");
        var savedPerson = registry.FindByUsername("ana");

        Assert.True(confirmed);
        Assert.False(secondConfirmation);
        Assert.NotNull(savedPerson);
        Assert.False(savedPerson.HasPendingLetter);
    }

    [Fact]
    public void MarkLetterPending_ReturnsFalse_WhenPersonDoesNotExist()
    {
        var registry = new CupidRegistry();

        var marked = registry.MarkLetterPending("ana");

        Assert.False(marked);
    }

    private static SinglePerson Person(string username) =>
        new(username, "Novi Sad", 24, "060123456");
}

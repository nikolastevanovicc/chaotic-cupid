using ChaoticCupid.Core.Models;
using ChaoticCupid.Core.Registry;
using Microsoft.AspNetCore.SignalR;

namespace ChaoticCupid.Server.Hubs;

public sealed class CupidHub : Hub
{
    private readonly CupidRegistry _registry;

    public CupidHub(CupidRegistry registry)
    {
        _registry = registry;
    }

    public Task<bool> InitSinglePerson(string username, string city, int age, string phoneNumber)
    {
        var person = new SinglePerson(username, city, age, phoneNumber);
        var registered = _registry.RegisterPerson(person);

        return Task.FromResult(registered);
    }

    public Task<bool> BlockUser(string username, string blockedUsername)
    {
        var blocked = _registry.BlockUser(username, blockedUsername);

        return Task.FromResult(blocked);
    }

    public Task<bool> ConfirmLetterReceived(string username)
    {
        var confirmed = _registry.ConfirmLetterReceived(username);

        return Task.FromResult(confirmed);
    }
}

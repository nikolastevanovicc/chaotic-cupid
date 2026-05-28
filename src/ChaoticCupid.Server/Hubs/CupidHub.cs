using ChaoticCupid.Core.Models;
using ChaoticCupid.Core.Registry;
using ChaoticCupid.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChaoticCupid.Server.Hubs;

public sealed class CupidHub : Hub
{
    private readonly CupidRegistry _registry;
    private readonly ClientConnectionStore _connectionStore;

    public CupidHub(CupidRegistry registry, ClientConnectionStore connectionStore)
    {
        _registry = registry;
        _connectionStore = connectionStore;
    }

    public Task<bool> InitSinglePerson(string username, string city, int age, string phoneNumber)
    {
        var person = new SinglePerson(username, city, age, phoneNumber);
        var registered = _registry.RegisterPerson(person);
        if (registered)
        {
            _connectionStore.Register(username, Context.ConnectionId);
        }

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

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connectionStore.RemoveByConnectionId(Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}

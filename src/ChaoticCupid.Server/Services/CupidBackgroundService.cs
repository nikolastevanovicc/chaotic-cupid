using ChaoticCupid.Core.Matching;
using ChaoticCupid.Core.Models;
using ChaoticCupid.Core.Registry;
using ChaoticCupid.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChaoticCupid.Server.Services;

public sealed class CupidBackgroundService : BackgroundService
{
    private static readonly TimeSpan SendInterval = TimeSpan.FromMinutes(1);
    private const string LoveLetterArrivedMethod = "LoveLetterArrived";

    private readonly CupidRegistry _registry;
    private readonly CupidMatcher _matcher;
    private readonly IRandomScoreProvider _randomScoreProvider;
    private readonly ClientConnectionStore _connectionStore;
    private readonly IHubContext<CupidHub> _hubContext;
    private readonly ILogger<CupidBackgroundService> _logger;

    public CupidBackgroundService(
        CupidRegistry registry,
        CupidMatcher matcher,
        IRandomScoreProvider randomScoreProvider,
        ClientConnectionStore connectionStore,
        IHubContext<CupidHub> hubContext,
        ILogger<CupidBackgroundService> logger)
    {
        _registry = registry;
        _matcher = matcher;
        _randomScoreProvider = randomScoreProvider;
        _connectionStore = connectionStore;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(SendInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await SendLoveLettersAsync(stoppingToken);
        }
    }

    private async Task SendLoveLettersAsync(CancellationToken cancellationToken)
    {
        var recipients = _registry.GetAvailableRecipients();

        foreach (var recipient in recipients)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await TrySendLoveLetterAsync(recipient, cancellationToken);
        }
    }

    private async Task TrySendLoveLetterAsync(SinglePerson recipient, CancellationToken cancellationToken)
    {
        if (!_connectionStore.TryGetConnectionId(recipient.Username, out var connectionId))
        {
            return;
        }

        var registeredRecipient = _registry.FindByUsername(recipient.Username);
        if (registeredRecipient is null)
        {
            return;
        }

        var candidates = _registry.GetCandidatesFor(recipient.Username);
        var match = _matcher.FindBestMatch(
            recipient,
            candidates,
            registeredRecipient.BlockedUsernames);

        if (match is null || !_registry.MarkLetterPending(recipient.Username))
        {
            return;
        }

        var letter = new LoveLetter(match.Sender, recipient, PickMessage());
        var notification = LoveLetterNotification.FromLoveLetter(letter);

        try
        {
            await _hubContext.Clients
                .Client(connectionId)
                .SendAsync(LoveLetterArrivedMethod, notification, cancellationToken);
        }
        catch (Exception ex)
        {
            _registry.ConfirmLetterReceived(recipient.Username);
            _logger.LogWarning(ex, "Failed to send love letter to {Username}.", recipient.Username);
        }
    }

    private CupidMessage PickMessage()
    {
        var values = Enum.GetValues<CupidMessage>();
        var index = _randomScoreProvider.NextInclusive(0, values.Length - 1);

        return values[index];
    }
}

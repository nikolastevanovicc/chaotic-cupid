using ChaoticCupid.Core.Matching;
using ChaoticCupid.Core.Registry;
using ChaoticCupid.Server.Hubs;
using ChaoticCupid.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<IRandomScoreProvider, CryptoRandomScoreProvider>();
builder.Services.AddSingleton<CupidRegistry>();
builder.Services.AddSingleton<CupidMatcher>();
builder.Services.AddSingleton<ClientConnectionStore>();
builder.Services.AddHostedService<CupidBackgroundService>();

var app = builder.Build();

app.MapGet("/", () => "Chaotic Cupid server is running.");
app.MapHub<CupidHub>("/cupidHub");

app.Run();

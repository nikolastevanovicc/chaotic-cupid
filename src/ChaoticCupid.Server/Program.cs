using ChaoticCupid.Core.Matching;
using ChaoticCupid.Core.Registry;
using ChaoticCupid.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<CupidRegistry>();
builder.Services.AddSingleton<CupidMatcher>();

var app = builder.Build();

app.MapGet("/", () => "Chaotic Cupid server is running.");
app.MapHub<CupidHub>("/cupidHub");

app.Run();

using Microsoft.AspNetCore.SignalR.Client;

const string DefaultHubUrl = "http://localhost:5000/cupidHub";

var username = ReadRequiredText("Username: ");
var city = ReadRequiredText("Grad: ");
var age = ReadNonNegativeInt("Godine: ");
var phoneNumber = ReadPhoneNumber("Broj telefona: ");
var hubUrl = ReadOptionalText($"SignalR hub URL [{DefaultHubUrl}]: ", DefaultHubUrl);

var connection = new HubConnectionBuilder()
    .WithUrl(hubUrl)
    .WithAutomaticReconnect()
    .Build();

await connection.StartAsync();

var registered = await connection.InvokeAsync<bool>(
    "InitSinglePerson",
    username,
    city,
    age,
    phoneNumber);

if (!registered)
{
    Console.WriteLine("Username vec postoji. Pokreni klijenta ponovo sa drugim username-om.");
    await connection.DisposeAsync();
    return;
}

Console.WriteLine("Uspesno ste prijavljeni za trazenje partnera.");
Console.WriteLine("Dostupne komande: /block username, /confirm, /exit");

while (true)
{
    var command = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(command))
    {
        continue;
    }

    if (command.Equals("/exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (command.Equals("/confirm", StringComparison.OrdinalIgnoreCase))
    {
        var confirmed = await connection.InvokeAsync<bool>("ConfirmLetterReceived", username);
        Console.WriteLine(confirmed
            ? "Potvrdili ste prijem pisma."
            : "Nema pisma koje ceka potvrdu.");
        continue;
    }

    if (command.StartsWith("/block ", StringComparison.OrdinalIgnoreCase))
    {
        var blockedUsername = command["/block ".Length..].Trim();
        if (string.IsNullOrWhiteSpace(blockedUsername))
        {
            Console.WriteLine("Unesite username koji zelite da blokirate.");
            continue;
        }

        var blocked = await connection.InvokeAsync<bool>("BlockUser", username, blockedUsername);
        Console.WriteLine(blocked
            ? $"Blokirali ste korisnika {blockedUsername}."
            : "Blokiranje nije uspelo.");
        continue;
    }

    Console.WriteLine("Nepoznata komanda. Dostupne komande: /block username, /confirm, /exit");
}

await connection.DisposeAsync();

static string ReadRequiredText(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var value = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(value))
        {
            return value.Trim();
        }

        Console.WriteLine("Vrednost ne sme biti prazna.");
    }
}

static string ReadOptionalText(string prompt, string defaultValue)
{
    Console.Write(prompt);
    var value = Console.ReadLine();

    return string.IsNullOrWhiteSpace(value)
        ? defaultValue
        : value.Trim();
}

static int ReadNonNegativeInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var value = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(value))
        {
            Console.WriteLine("Vrednost ne sme biti prazna.");
            continue;
        }

        if (!int.TryParse(value, out var number))
        {
            Console.WriteLine("Unesite broj, ne karaktere.");
            continue;
        }

        if (number < 0)
        {
            Console.WriteLine("Broj ne sme biti negativan.");
            continue;
        }

        return number;
    }
}

static string ReadPhoneNumber(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var value = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(value))
        {
            Console.WriteLine("Vrednost ne sme biti prazna.");
            continue;
        }

        value = value.Trim();
        if (value.StartsWith('-'))
        {
            Console.WriteLine("Broj telefona ne sme biti negativan.");
            continue;
        }

        if (!value.All(char.IsDigit))
        {
            Console.WriteLine("Broj telefona sme da sadrzi samo cifre.");
            continue;
        }

        return value;
    }
}

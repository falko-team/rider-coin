using Talkie.Common;

namespace Falko.Coin.Bot.Configurations;

public static class BotConfiguration
{
    public static string GetTelegramBotToken()
    {
        var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

        ArgumentException.ThrowIfNullOrWhiteSpace(token, nameof(token));

        return token;
    }

    public static string GetWalletsStorageDirectory()
    {
        var directory = Environment.GetEnvironmentVariable("WALLETS_STORAGE_DIRECTORY");

        return directory.IsNullOrWhiteSpace() is false
            ? directory!
            : Path.Combine(Environment.CurrentDirectory, "Wallets");
    }
}

using Talkie.Common;
using Talkie.Validations;

namespace Falko.Coin.Bot.Configurations;

public static class BotConfiguration
{
    public static string GetTelegramBotToken()
    {
        var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

        token.ThrowIf()!.NullOrWhiteSpace();

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

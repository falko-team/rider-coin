using Falko.Coin.Bot.Behaviors;
using Falko.Coin.Bot.Configurations;
using Falko.Coin.Bot.Integrations;
using Falko.Coin.Wallets.Services;
using Falko.Coin.Wallets.Storages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Talkie.Hosting;

await new HostBuilder()
    .ConfigureAppConfiguration(configuration => configuration
        .AddEnvironmentVariables())
    .UseSerilog((_, configuration) => configuration
        .MinimumLevel.Verbose()
        .WriteTo.Console())
    .UseTalkie()
    .ConfigureServices(services => services
        .AddIntegrations<TelegramSubscriber>()
        .AddBehaviors<StartOrCreateSubscriber>()
        .AddBehaviors<WalletSubscriber>()
        .AddBehaviors<BalanceSubscriber>()
        .AddBehaviors<TransferSubscriber>()
        .AddBehaviors<PrivacySubscriber>())
    .ConfigureServices(services => services
        .AddSingleton<IWalletsStorage, WalletsStorage>(_ => new WalletsStorage(BotConfiguration.GetWalletsStorageDirectory()))
        .AddSingleton<IWalletsPool, WalletsPool>())
    .RunConsoleAsync();

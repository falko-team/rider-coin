using Falko.Coin.Bot.Configurations;
using Falko.Coin.Bot.Extensions;
using Falko.Coin.Bot.Subscriptors;
using Falko.Coin.Wallets.Services;
using Falko.Coin.Wallets.Storages;
using Talkie.Collections;
using Talkie.Disposables;
using Talkie.Flows;
using Talkie.Pipelines;

// preparing
await using var disposables = new DisposableStack();

// configuring
var collection = new ServiceCollection();

collection.AddLogging(builder => builder
    .AddSimpleConsole(options => options.SingleLine = true)
    .SetMinimumLevel(LogLevel.Trace));

collection.AddSingleton<IWalletsStorage, WalletsStorage>(_ => new WalletsStorage(BotConfiguration.GetWalletsStorageDirectory()));
collection.AddSingleton<IWalletsPool, WalletsPool>();

collection.AddSubscriptor<TransferSubscriptor>();
collection.AddSubscriptor<BalanceSubscriptor>();
collection.AddSubscriptor<WalletSubscriptor>();
collection.AddSubscriptor<StartOrCreateSubscriptor>();
collection.AddSubscriptor<PrivacySubscriptor>();

var provider = collection.BuildServiceProvider()
    .DisposeWith(disposables);

var logger = provider.GetRequiredService<ILogger<SignalFlow>>();

logger.LogInformation("Signal flow is initializing");

// initializing
var flow = new SignalFlow()
    .DisposeWith(disposables);

var unobservedExceptionTask = flow.TakeUnobservedExceptionAsync();

flow.Subscribe(signals => signals
    .Handle(context => logger.LogTrace(context.Signal.ToString())));

flow.Subscribe<TransferSubscriptor>(provider);
flow.Subscribe<BalanceSubscriptor>(provider);
flow.Subscribe<WalletSubscriptor>(provider);
flow.Subscribe<StartOrCreateSubscriptor>(provider);
flow.Subscribe<PrivacySubscriptor>(provider);

logger.LogInformation("Signal flow is running");

// running
await flow.ConnectTelegramAsync(BotConfiguration.GetTelegramBotToken())
    .DisposeAsyncWith(disposables);

logger.LogInformation("Signal flow is processing");

// waiting
throw await unobservedExceptionTask;

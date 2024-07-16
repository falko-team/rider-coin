using Falko.Coin.Bot.Extensions;
using Falko.Coin.Wallets.Services;
using Talkie.Controllers;
using Talkie.Flows;
using Talkie.Handlers;
using Talkie.Models.Profiles;
using Talkie.Pipelines;
using Talkie.Signals;

namespace Falko.Coin.Bot.Subscriptors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class StartOrCreateSubscriptor(IWalletsPool wallets, ILogger<StartOrCreateSubscriptor> logger) : ISubscriptor
{
    public void Subscribe(ISignalFlow flow)
    {
        var skipOlderThanMinutePipeline = new SignalInterceptingPipelineBuilder<IncomingMessageSignal>()
            .SkipOlderThan(TimeSpan.FromMinutes(1));

        flow.Subscribe(skipOlderThanMinutePipeline, signals => signals
            .Where(signal => signal.Message.EnvironmentProfile is IUserProfile)
            .OnlyCommand("start", logger)
            .Merge(skipOlderThanMinutePipeline, mergingSignals => mergingSignals
                .Where(signal => signal.Message.EnvironmentProfile is not IUserProfile)
                .OnlyCommand("create", logger))
            .HandleAsync(TryCreateProfileWallet));
    }

    private async ValueTask TryCreateProfileWallet(ISignalContext<IncomingMessageSignal> context,
        CancellationToken cancellationToken)
    {
        if (context.TryGetWalletIdentifier(out var walletIdentifier, logger) is false)
        {
            return;
        }

        if (wallets.TryGet(walletIdentifier, out var wallet))
        {
            logger.LogInformation($"User with {context.Signal.Message.SenderProfile.Identifier} has wallet with identifier {wallet.Identifier}");

            await context
                .ToMessageController()
                .PublishMessageAsync(message => message
                    .AddTextLine("У вас уже есть кошелек"), cancellationToken);
        }
        else
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has no wallet");

            if (wallets.TryAdd(walletIdentifier, out wallet))
            {
                wallet.Deposit(100);
            }
            else
            {
                wallets.TryGet(walletIdentifier, out wallet);
            }

            logger.LogInformation($"Created wallet with user identifier {walletIdentifier}");

            await context
                .ToMessageController()
                .PublishMessageAsync("Ваш кошелек создан",
                    cancellationToken);
        }
    }
}

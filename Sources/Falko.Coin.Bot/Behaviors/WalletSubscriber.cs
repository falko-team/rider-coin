using Falko.Coin.Bot.Extensions;
using Falko.Coin.Wallets.Services;
using Microsoft.Extensions.Logging;
using Talkie.Controllers.MessageControllers;
using Talkie.Disposables;
using Talkie.Flows;
using Talkie.Handlers;
using Talkie.Pipelines.Handling;
using Talkie.Pipelines.Intercepting;
using Talkie.Signals;
using Talkie.Subscribers;

namespace Falko.Coin.Bot.Behaviors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class WalletSubscriber(IWalletsPool wallets, ILogger<BalanceSubscriber> logger) : IBehaviorsSubscriber
{
    public void Subscribe(ISignalFlow flow, IRegisterOnlyDisposableScope disposables, CancellationToken cancellationToken)
    {
        flow.Subscribe<MessagePublishedSignal>(signals => signals
            .SkipSelfPublished()
            .SkipOlderThan(TimeSpan.FromMinutes(1))
            .SelectOnlyCommand("wallet", logger)
            .HandleAsync(SendProfileWallet));
    }

    private async ValueTask SendProfileWallet
    (
        ISignalContext<MessagePublishedSignal> context,
        CancellationToken cancellationToken
    )
    {
        if (context.TryGetWalletIdentifier(out var walletIdentifier, logger) is false)
        {
            return;
        }

        if (wallets.TryGet(walletIdentifier, out var wallet))
        {
            logger.LogInformation
            (
                "User with {Publisher} has wallet with identifier {WalletIdentifier}",
                context.Signal.Message.PublisherProfile.Identifier,
                wallet.Identifier
            );

            await context
                .ToMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletAddress
                    .WithUserPublisherProfile(context)
                    .WithWalletAddress(wallet), cancellationToken);
        }
        else
        {
            logger.LogWarning
            (
                "User with {Publisher} has no wallet",
                context.Signal.Message.PublisherProfile.Identifier
            );

            await context
                .ToMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletIsMissing
                    .WithUserPublisherProfile(context), cancellationToken);
        }
    }
}

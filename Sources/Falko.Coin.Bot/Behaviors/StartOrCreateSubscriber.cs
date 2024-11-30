using Falko.Coin.Bot.Extensions;
using Falko.Coin.Wallets.Services;
using Microsoft.Extensions.Logging;
using Talkie.Common;
using Talkie.Controllers.MessageControllers;
using Talkie.Disposables;
using Talkie.Flows;
using Talkie.Handlers;
using Talkie.Models.Messages;
using Talkie.Models.Profiles;
using Talkie.Pipelines.Handling;
using Talkie.Pipelines.Intercepting;
using Talkie.Signals;
using Talkie.Subscribers;

namespace Falko.Coin.Bot.Behaviors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class StartOrCreateSubscriber(IWalletsPool wallets, ILogger<StartOrCreateSubscriber> logger) : IBehaviorsSubscriber
{
    public void Subscribe
    (
        ISignalFlow flow,
        IRegisterOnlyDisposableScope disposables,
        CancellationToken cancellationToken
    )
    {
        flow.Subscribe<MessagePublishedSignal>(signals => signals
            .SkipSelfPublished()
            .SkipOlderThan(TimeSpan.FromMinutes(1))
            .Merge
            (
                mergeSignals => mergeSignals
                    .Where(signal => signal.Message.EnvironmentProfile is IUserProfile)
                    .SelectOnlyCommand("start", logger),
                mergeSignals => mergeSignals
                    .SelectOnlyCommand("create", logger)
            )
            .Where(signal => signal.Message.GetText().IsNullOrEmpty())
            .HandleAsync(TryCreateProfileWallet));
    }

    private async ValueTask TryCreateProfileWallet
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
                    .WalletIsExists
                    .WithWalletAddress(wallet)
                    .WithUserPublisherProfile(context), cancellationToken);
        }
        else
        {
            logger.LogWarning
            (
                "User with {Publisher} has no wallet",
                context.Signal.Message.PublisherProfile.Identifier
            );

            if (wallets.TryAdd(walletIdentifier, out wallet))
            {
                wallet.Deposit(100);
            }
            else
            {
                wallets.TryGet(walletIdentifier, out wallet);
            }

            logger.LogInformation
            (
                "Created wallet with user identifier {WalletIdentifier}",
                walletIdentifier
            );

            await context
                .ToMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletIsCreated
                    .WithWalletAddress(wallet!)
                    .WithUserPublisherProfile(context), cancellationToken);
        }
    }
}

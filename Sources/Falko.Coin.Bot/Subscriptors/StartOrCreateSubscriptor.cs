using Falko.Coin.Bot.Extensions;
using Falko.Coin.Wallets.Services;
using Talkie.Common;
using Talkie.Controllers.OutgoingMessageControllers;
using Talkie.Flows;
using Talkie.Handlers;
using Talkie.Models.Profiles;
using Talkie.Pipelines.Handling;
using Talkie.Pipelines.Intercepting;
using Talkie.Signals;

namespace Falko.Coin.Bot.Subscriptors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class StartOrCreateSubscriptor(IWalletsPool wallets, ILogger<StartOrCreateSubscriptor> logger) : ISubscriptor
{
    public void Subscribe(ISignalFlow flow)
    {
        flow.Subscribe<IncomingMessageSignal>(signals => signals
            .SkipSelfSent()
            .SkipOlderThan(TimeSpan.FromMinutes(1))
            .Merge
            (
                mergeSignals => mergeSignals
                    .Where(signal => signal.Message.EnvironmentProfile is IUserProfile)
                    .SelectOnlyCommand("start", logger),
                mergeSignals => mergeSignals
                    .SelectOnlyCommand("create", logger)
            )
            .Where(signal => signal.Message.Content.Text.IsNullOrEmpty())
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
                .ToOutgoingMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletIsExists
                    .WithWalletAddress(wallet), cancellationToken);
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
                .ToOutgoingMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletIsCreated
                    .WithWalletAddress(wallet!)
                    .WithSenderProfileUser(context), cancellationToken);
        }
    }
}

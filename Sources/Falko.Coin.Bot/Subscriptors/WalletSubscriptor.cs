using Falko.Coin.Bot.Extensions;
using Falko.Coin.Wallets.Services;
using Talkie.Controllers.OutgoingMessageControllers;
using Talkie.Flows;
using Talkie.Handlers;
using Talkie.Pipelines.Handling;
using Talkie.Pipelines.Intercepting;
using Talkie.Signals;

namespace Falko.Coin.Bot.Subscriptors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class WalletSubscriptor(IWalletsPool wallets, ILogger<BalanceSubscriptor> logger) : ISubscriptor
{
    public void Subscribe(ISignalFlow flow)
    {
        flow.Subscribe<IncomingMessageSignal>(signals => signals
            .SkipSelfSent()
            .SkipOlderThan(TimeSpan.FromMinutes(1))
            .SelectOnlyCommand("wallet", logger)
            .HandleAsync(SendProfileWallet));
    }

    private async ValueTask SendProfileWallet(ISignalContext<IncomingMessageSignal> context,
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
                    .WalletIsMissing
                    .WithSenderProfileUser(context)
                    .WithWalletAddress(wallet), cancellationToken);
        }
        else
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has no wallet");

            await context
                .ToOutgoingMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletIsMissing
                    .WithSenderProfileUser(context), cancellationToken);
        }
    }
}

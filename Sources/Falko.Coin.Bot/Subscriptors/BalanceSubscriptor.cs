using Falko.Coin.Bot.Extensions;
using Falko.Coin.Wallets.Services;
using Talkie.Controllers;
using Talkie.Flows;
using Talkie.Handlers;
using Talkie.Pipelines;
using Talkie.Pipelines.Handling;
using Talkie.Pipelines.Intercepting;
using Talkie.Signals;

namespace Falko.Coin.Bot.Subscriptors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class BalanceSubscriptor(IWalletsPool wallets, ILogger<BalanceSubscriptor> logger) : ISubscriptor
{
    public void Subscribe(ISignalFlow flow)
    {
        flow.Subscribe<IncomingMessageSignal>(signals => signals
            .SkipOlderThan(TimeSpan.FromMinutes(1))
            .SelectOnlyCommand("balance", logger)
            .Do(signal => logger.LogDebug($"Balance command text: {signal.Message.Text}"))
            .HandleAsync(SendProfileBalance));
    }

    private async ValueTask SendProfileBalance(ISignalContext<IncomingMessageSignal> context,
        CancellationToken cancellationToken)
    {
        if (context.TryGetWalletIdentifier(out var walletIdentifier, logger) is false)
        {
            return;
        }

        if (wallets.TryGet(walletIdentifier, out var wallet))
        {
            logger.LogInformation($"User with {context.Signal.Message.SenderProfile.Identifier} has wallet with balance {wallet.Balance}");

            await context
                .ToOutgoingMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletBalance
                    .WithWalletBalance(wallet)
                    .WithSenderProfileUser(context), cancellationToken: cancellationToken);
        }
        else
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has no wallet");

            await context
                .ToOutgoingMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletIsMissing
                    .WithSenderProfileUser(context), cancellationToken: cancellationToken);
        }
    }
}

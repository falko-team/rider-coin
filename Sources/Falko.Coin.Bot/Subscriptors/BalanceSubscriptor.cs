using Falko.Coin.Bot.Configurations;
using Falko.Coin.Bot.Extensions;
using Falko.Coin.Wallets.Services;
using Talkie.Controllers;
using Talkie.Flows;
using Talkie.Handlers;
using Talkie.Pipelines;
using Talkie.Signals;

namespace Falko.Coin.Bot.Subscriptors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class BalanceSubscriptor(IWalletsPool wallets, ILogger<BalanceSubscriptor> logger) : ISubscriptor
{
    public void Subscribe(ISignalFlow flow)
    {
        flow.Subscribe<IncomingMessageSignal>(signals => signals
            .SkipOlderThan(TimeSpan.FromMinutes(1))
            .OnlyCommand("balance", logger)
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
                .ToMessageController()
                .PublishMessageAsync($"Ваш баланс - {wallet.Balance} {BotConfiguration.CoinName}",
                    cancellationToken);
        }
        else
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has no wallet");

            await context
                .ToMessageController()
                .PublishMessageAsync("У вас нет кошелька",
                    cancellationToken);
        }
    }
}

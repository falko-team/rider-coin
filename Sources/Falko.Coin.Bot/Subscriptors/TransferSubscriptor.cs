using Falko.Coin.Bot.Configurations;
using Falko.Coin.Bot.Extensions;
using Falko.Coin.Wallets.Services;
using Falko.Coin.Wallets.Transactions;
using Talkie.Controllers;
using Talkie.Flows;
using Talkie.Handlers;
using Talkie.Pipelines;
using Talkie.Signals;

namespace Falko.Coin.Bot.Subscriptors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class TransferSubscriptor(IWalletsPool wallets, ILogger<TransferSubscriptor> logger) : ISubscriptor
{
    public void Subscribe(ISignalFlow flow)
    {
        flow.Subscribe<IncomingMessageSignal>(signals => signals
            .SkipOlderThan(TimeSpan.FromMinutes(1))
            .OnlyCommand("transfer")
            .Do(signal => logger.LogInformation($"Transfer command text: {signal.Message.Text}"))
            .HandleAsync(TryTransferProfileWalletAmountToOther));
    }

    private async ValueTask TryTransferProfileWalletAmountToOther(ISignalContext<IncomingMessageSignal> context,
        CancellationToken cancellationToken)
    {
        if (context.TryGetWalletIdentifier(out var walletIdentifier, logger) is false)
        {
            return;
        }

        if (wallets.TryGet(walletIdentifier, out var wallet) is false)
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has no wallet");

            await context
                .ToMessageController()
                .PublishMessageAsync("У вас нет кошелька",
                    cancellationToken);

            return;
        }

        var commandArguments = context.GetCommandArguments();

        if (commandArguments.Count < 2)
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has no command arguments");

            await context
                .ToMessageController()
                .PublishMessageAsync("Укажите сумму и идентификатор кошелька получателя",
                    cancellationToken);

            return;
        }

        if (long.TryParse(commandArguments[0], out var amount) is false)
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has invalid amount");

            await context
                .ToMessageController()
                .PublishMessageAsync("Укажите корректную сумму",
                    cancellationToken);

            return;
        }

        if (long.TryParse(commandArguments[1], out var recipientWalletIdentifier) is false)
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has invalid recipient wallet identifier");

            await context
                .ToMessageController()
                .PublishMessageAsync("Укажите корректный идентификатор кошелька получателя",
                    cancellationToken);

            return;
        }

        if (wallets.TryGet(recipientWalletIdentifier, out var recipientWallet) is false)
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has no recipient wallet");

            await context
                .ToMessageController()
                .PublishMessageAsync("Кошелек получателя не найден",
                    cancellationToken);

            return;
        }

        if (WalletTransaction.TryTransfer(wallet, recipientWallet, amount) is false)
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has not enough money");

            await context
                .ToMessageController()
                .PublishMessageAsync("Недостаточно средств",
                    cancellationToken);

            return;
        }

        logger.LogInformation($"User with {context.Signal.Message.SenderProfile.Identifier} transferred {amount} to {recipientWallet.Identifier}");

        await context
            .ToMessageController()
            .PublishMessageAsync($"Вы перевели {amount} {BotConfiguration.CoinName} на кошелек {recipientWallet.Identifier}",
                cancellationToken);

        try
        {
            await context
                .CreateMessageController(walletIdentifier)
                .PublishMessageAsync($"C вашего кошелька списано {amount} {BotConfiguration.CoinName} на кошелек {recipientWallet.Identifier}",
                    cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to notify sender about transfer");
        }

        try
        {
            await context
                .CreateMessageController(recipientWalletIdentifier)
                .PublishMessageAsync($"На ваш кошелек поступило {amount} {BotConfiguration.CoinName} от кошелька {wallet.Identifier}",
                    cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to notify recipient about transfer");
        }
    }
}

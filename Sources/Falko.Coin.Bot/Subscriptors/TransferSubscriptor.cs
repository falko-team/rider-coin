using Falko.Coin.Bot.Extensions;
using Falko.Coin.Wallets.Services;
using Falko.Coin.Wallets.Transactions;
using Talkie.Controllers;
using Talkie.Flows;
using Talkie.Handlers;
using Talkie.Pipelines.Handling;
using Talkie.Pipelines.Intercepting;
using Talkie.Signals;

namespace Falko.Coin.Bot.Subscriptors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class TransferSubscriptor(IWalletsPool wallets, ILogger<TransferSubscriptor> logger) : ISubscriptor
{
    public void Subscribe(ISignalFlow flow)
    {
        flow.Subscribe<IncomingMessageSignal>(signals => signals
            .SkipOlderThan(TimeSpan.FromMinutes(1))
            .SelectOnlyCommand("transfer")
            .Do(signal => logger.LogDebug($"Transfer command text: {signal.Message.Text}"))
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
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletIsMissing
                    .WithSenderProfileUser(context), cancellationToken);

            return;
        }

        var commandArguments = context.GetCommandArguments();

        if (commandArguments.Count < 2)
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has no command arguments");

            await context
                .ToMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .RequiredAmountAndWalletAddress, cancellationToken);

            return;
        }

        if (long.TryParse(commandArguments[0], out var amount) is false)
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has invalid amount");

            await context
                .ToMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletIsMissing
                    .WithAmount(commandArguments[0]), cancellationToken);

            return;
        }

        if (long.TryParse(commandArguments[1], out var recipientWalletIdentifier) is false)
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has invalid recipient wallet identifier");

            await context
                .ToMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletAddressIsInvalid
                    .WithAddress(commandArguments[1]), cancellationToken);

            return;
        }

        if (wallets.TryGet(recipientWalletIdentifier, out var recipientWallet) is false)
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has no recipient wallet");

            await context
                .ToMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletAddressIsMissing
                    .WithAddress(recipientWalletIdentifier), cancellationToken);

            return;
        }

        if (WalletTransaction.TryTransfer(wallet, recipientWallet, amount) is false)
        {
            logger.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has not enough money");

            await context
                .ToMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletBalanceTooLow
                    .WithSenderProfileUser(context), cancellationToken);

            return;
        }

        await context
            .ToMessageController()
            .PublishMessageAsync(context
                .GetLocalization()
                .TransactionProcessing
                .WithSenderProfileUser(context), cancellationToken);

        logger.LogInformation($"User with {context.Signal.Message.SenderProfile.Identifier} transferred {amount} to {recipientWallet.Identifier}");

        await context
            .ToMessageController()
            .PublishMessageAsync(context
                .GetLocalization()
                .TransactionProcessed
                .WithSenderProfileUser(context)
                .WithWalletAddress(recipientWallet)
                .WithAmount(amount), cancellationToken);

        try
        {
            await context
                .CreateMessageController(walletIdentifier)
                .PublishMessageAsync(context
                    .GetLocalization()
                    .TransactionSent
                    .WithWalletAddress(recipientWallet)
                    .WithAmount(amount), cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to notify sender about transfer");
        }

        try
        {
            await context
                .CreateMessageController(recipientWalletIdentifier)
                .PublishMessageAsync(context
                    .GetLocalization()
                    .TransactionReceived
                    .WithWalletAddress(recipientWallet)
                    .WithAmount(amount), cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to notify recipient about transfer");
        }
    }
}

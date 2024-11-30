using Falko.Coin.Bot.Extensions;
using Falko.Coin.Wallets.Services;
using Falko.Coin.Wallets.Transactions;
using Microsoft.Extensions.Logging;
using Talkie.Controllers.MessageControllers;
using Talkie.Disposables;
using Talkie.Flows;
using Talkie.Handlers;
using Talkie.Models.Messages;
using Talkie.Pipelines.Handling;
using Talkie.Pipelines.Intercepting;
using Talkie.Signals;
using Talkie.Subscribers;

namespace Falko.Coin.Bot.Behaviors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class TransferSubscriber(IWalletsPool wallets, ILogger<TransferSubscriber> logger) : IBehaviorsSubscriber
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
            .SelectOnlyCommand("transfer")
            .Do(signal => logger.LogDebug("Transfer command text: {Text}", signal.Message.GetText()))
            .HandleAsync(TryTransferProfileWalletAmountToOther));
    }

    private async ValueTask TryTransferProfileWalletAmountToOther
    (
        ISignalContext<MessagePublishedSignal> context,
        CancellationToken cancellationToken
    )
    {
        if (context.TryGetWalletIdentifier(out var walletIdentifier, logger) is false)
        {
            return;
        }

        if (wallets.TryGet(walletIdentifier, out var wallet) is false)
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

            return;
        }

        var commandArguments = context.GetCommandArguments();

        if (commandArguments.Count < 2)
        {
            logger.LogWarning
            (
                "User with {Publisher} has no command arguments",
                context.Signal.Message.PublisherProfile.Identifier
            );

            await context
                .ToMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .RequiredAmountAndWalletAddress, cancellationToken);

            return;
        }

        if (long.TryParse(commandArguments[0], out var amount) is false)
        {
            logger.LogWarning
            (
                "User with {Publisher} has invalid amount",
                context.Signal.Message.PublisherProfile.Identifier
            );

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
            logger.LogWarning
            (
                "User with {Publisher} has invalid recipient wallet identifier",
                context.Signal.Message.PublisherProfile.Identifier
            );

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
            logger.LogWarning
            (
                "User with {Publisher} has no recipient wallet",
                context.Signal.Message.PublisherProfile.Identifier
            );

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
            logger.LogWarning
            (
                "User with {Publisher} has not enough money",
                context.Signal.Message.PublisherProfile.Identifier
            );

            await context
                .ToMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .WalletBalanceTooLow
                    .WithUserPublisherProfile(context), cancellationToken);

            return;
        }

        await context
            .ToMessageController()
            .PublishMessageAsync(context
                .GetLocalization()
                .TransactionProcessing
                .WithUserPublisherProfile(context), cancellationToken);

        logger.LogInformation
        (
            "User with {Publisher} transferred {Amount} to {RecipientWalletIdentifier}",
            context.Signal.Message.PublisherProfile.Identifier,
            amount,
            recipientWallet.Identifier
        );

        try
        {
            await context
                .ToMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .TransactionProcessed
                    .WithUserPublisherProfile(context)
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
                .GetMessageController(walletIdentifier)
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
                .GetMessageController(recipientWalletIdentifier)
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

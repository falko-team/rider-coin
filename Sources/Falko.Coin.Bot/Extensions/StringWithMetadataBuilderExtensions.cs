using Falko.Coin.Bot.Localizations;
using Falko.Coin.Wallets.Wallets;
using Talkie.Handlers;
using Talkie.Signals;

namespace Falko.Coin.Bot.Extensions;

public static class StringWithMetadataBuilderExtensions
{
    public static StringWithMetadataBuilder WithWalletBalance(this StringWithMetadataBuilder stringWithMetadataBuilder,
        IWallet wallet)
    {
        return stringWithMetadataBuilder.WithAmount(wallet
            .Balance);
    }

    public static StringWithMetadataBuilder WithWalletAddress(this StringWithMetadataBuilder stringWithMetadataBuilder,
        IWallet wallet)
    {
        return stringWithMetadataBuilder.WithAddress(wallet
            .Identifier);
    }

    public static StringWithMetadataBuilder WithSenderProfileUser(this StringWithMetadataBuilder stringWithMetadataBuilder,
        ISignalContext<IncomingMessageSignal> context)
    {
        return stringWithMetadataBuilder.WithUser(context
            .Signal
            .Message
            .SenderProfile
            .GetDisplayName());
    }

    public static StringWithMetadataBuilder WithEnvironmentProfileBot(this StringWithMetadataBuilder stringWithMetadataBuilder,
        ISignalContext<IncomingMessageSignal> context)
    {
        return stringWithMetadataBuilder.WithBot(context
            .Signal
            .Message
            .ReceiverProfile
            .GetDisplayName());
    }
}

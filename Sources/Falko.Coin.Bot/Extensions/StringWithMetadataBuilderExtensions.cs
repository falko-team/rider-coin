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

    public static StringWithMetadataBuilder WithUserPublisherProfile(this StringWithMetadataBuilder stringWithMetadataBuilder,
        ISignalContext<MessagePublishedSignal> context)
    {
        return stringWithMetadataBuilder.WithUser(context
            .Signal
            .Message
            .PublisherProfile
            .GetDisplayName());
    }

    public static StringWithMetadataBuilder WithBotEnvironmentProfile(this StringWithMetadataBuilder stringWithMetadataBuilder,
        ISignalContext<MessagePublishedSignal> context)
    {
        return stringWithMetadataBuilder.WithBot(context
            .Signal
            .Message
            .ReceiverProfile
            .GetDisplayName());
    }
}

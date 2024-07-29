namespace Falko.Coin.Bot.Localizations;

public interface ILocalization
{
    StringWithMetadataBuilder WalletIsMissing { get; }

    StringWithMetadataBuilder WalletIsExists { get; }

    StringWithMetadataBuilder WalletIsCreated { get; }

    StringWithMetadataBuilder WalletBalance { get; }

    StringWithMetadataBuilder WalletAddress { get; }

    StringWithMetadataBuilder WalletBalanceTooLow { get; }

    StringWithMetadataBuilder RequiredAmountAndWalletAddress { get; }

    StringWithMetadataBuilder AmountIsIncorrect { get; }

    StringWithMetadataBuilder WalletAddressIsMissing { get; }

    StringWithMetadataBuilder WalletAddressIsInvalid { get; }

    StringWithMetadataBuilder TransactionProcessing { get; }

    StringWithMetadataBuilder TransactionProcessed { get; }

    StringWithMetadataBuilder TransactionReceived { get; }

    StringWithMetadataBuilder TransactionSent { get; }

    StringWithMetadataBuilder BotPrivacyPolicy { get; }
}

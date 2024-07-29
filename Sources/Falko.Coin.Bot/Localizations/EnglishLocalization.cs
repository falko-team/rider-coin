using System.Text;

namespace Falko.Coin.Bot.Localizations;

public sealed class EnglishLocalization : ILocalization
{
    public StringWithMetadataBuilder WalletIsMissing =>
        $"The {StringMetadata.User} don't have a wallet";

    public StringWithMetadataBuilder WalletIsExists =>
        $"The {StringMetadata.User} already has a wallet";

    public StringWithMetadataBuilder WalletIsCreated =>
        $"The {StringMetadata.User} wallet was created with the {StringMetadata.Address} address";

    public StringWithMetadataBuilder WalletBalance =>
        $"The {StringMetadata.User} wallet has {StringMetadata.Amount} {StringMetadata.Coin}";

    public StringWithMetadataBuilder WalletAddress =>
        $"The {StringMetadata.User} wallet address is {StringMetadata.Address}";

    public StringWithMetadataBuilder WalletBalanceTooLow =>
        $"The {StringMetadata.User} wallet has insufficient {StringMetadata.Coin}";

    public StringWithMetadataBuilder RequiredAmountAndWalletAddress =>
        "The amount and receiver wallet address are required";

    public StringWithMetadataBuilder AmountIsIncorrect =>
        $"The amount '{StringMetadata.Amount}' format is incorrect";

    public StringWithMetadataBuilder WalletAddressIsMissing =>
        $"The wallet with the {StringMetadata.Address} address does not exist";

    public StringWithMetadataBuilder WalletAddressIsInvalid =>
        $"The '{StringMetadata.Address}' wallet address format is invalid";

    public StringWithMetadataBuilder TransactionProcessing =>
        $"The {StringMetadata.User} transaction is processing";

    public StringWithMetadataBuilder TransactionProcessed =>
        $"The {StringMetadata.User} transfer {StringMetadata.Amount} {StringMetadata.Coin} to the {StringMetadata.Address} wallet";

    public StringWithMetadataBuilder TransactionReceived =>
        $"You received {StringMetadata.Amount} {StringMetadata.Coin} from the {StringMetadata.Address} wallet";

    public StringWithMetadataBuilder TransactionSent =>
        $"You sent {StringMetadata.Amount} {StringMetadata.Coin} to the {StringMetadata.Address} wallet";

    public StringWithMetadataBuilder BotPrivacyPolicy => new StringBuilder()
        .AppendLine("Privacy Policy")
        .AppendLine()
        .AppendLine($"Bot - the {StringMetadata.Bot} bot.")
        .AppendLine($"We - the {StringMetadata.Team}, the bot developers.")
        .AppendLine($"You - the {StringMetadata.User} user of the bot.")
        .AppendLine()
        .AppendLine("We collect a minimum amount of information about you to ensure the bot's operation:")
        .AppendLine("- User ID: for wallet identification.")
        .AppendLine("- Wallet information: to perform operations with it.")
        .AppendLine("- Message logs: only those that start with '/'.")
        .AppendLine("- Action logs: only those you perform.")
        .AppendLine()
        .AppendLine("Why we do this:")
        .AppendLine("- To ensure the bot's operation.")
        .AppendLine()
        .AppendLine("Your data is safe:")
        .AppendLine("- We do not share data with third parties.")
        .AppendLine("- We do not use data for commercial purposes.")
        .AppendLine("- We do not store data longer than necessary.")
        .AppendLine()
        .AppendLine("Your rights:")
        .AppendLine("- You can request data deletion.")
        .AppendLine("- You can request data export.")
        .AppendLine()
        .AppendLine("Policy changes:")
        .AppendLine("- We may make changes to the privacy policy at any time.")
        .AppendLine()
        .AppendLine("Contact us:")
        .AppendLine("- For any questions, please write to: fembina@pm.me.")
        .AppendLine()
        .AppendLine("Consent:")
        .AppendLine("- By using the bot, you agree to our privacy policy.");
}

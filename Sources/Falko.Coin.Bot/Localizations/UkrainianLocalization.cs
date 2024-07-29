using System.Text;

namespace Falko.Coin.Bot.Localizations;

public sealed class UkrainianLocalization : ILocalization
{
    public StringWithMetadataBuilder WalletIsMissing =>
        $"У {StringMetadata.User} немає гаманця";

    public StringWithMetadataBuilder WalletIsExists =>
        $"У {StringMetadata.User} вже є гаманець";

    public StringWithMetadataBuilder WalletIsCreated =>
        $"Створено гаманець для {StringMetadata.User} з адресою {StringMetadata.Address}";

    public StringWithMetadataBuilder WalletBalance =>
        $"У {StringMetadata.User} на гаманці {StringMetadata.Amount} {StringMetadata.Coin}";

    public StringWithMetadataBuilder WalletAddress =>
        $"Адреса гаманця {StringMetadata.User} - {StringMetadata.Address}";

    public StringWithMetadataBuilder WalletBalanceTooLow =>
        $"У {StringMetadata.User} на гаманці недостатньо {StringMetadata.Coin}";

    public StringWithMetadataBuilder RequiredAmountAndWalletAddress =>
        "Потрібно вказати суму та адресу гаманця";

    public StringWithMetadataBuilder AmountIsIncorrect =>
        $"Формат суми '{StringMetadata.Amount}' невірний";

    public StringWithMetadataBuilder WalletAddressIsMissing =>
        $"Гаманця з адресою {StringMetadata.Address} не існує";

    public StringWithMetadataBuilder WalletAddressIsInvalid =>
        $"Формат гаманця з адресою '{StringMetadata.Address}' невірний";

    public StringWithMetadataBuilder TransactionProcessing =>
        $"Транзакція користувача {StringMetadata.User} обробляється";

    public StringWithMetadataBuilder TransactionProcessed =>
        $"Користувач {StringMetadata.User} переказав {StringMetadata.Amount} {StringMetadata.Coin} на гаманець {StringMetadata.Address}";

    public StringWithMetadataBuilder TransactionReceived =>
        $"Ви отримали {StringMetadata.Amount} {StringMetadata.Coin} від гаманця {StringMetadata.Address}";

    public StringWithMetadataBuilder TransactionSent =>
        $"Ви відправили {StringMetadata.Amount} {StringMetadata.Coin} на гаманець {StringMetadata.Address}";

    public StringWithMetadataBuilder BotPrivacyPolicy => new StringBuilder()
        .AppendLine("Політика конфіденційності")
        .AppendLine()
        .AppendLine($"Бот - {StringMetadata.Bot} бот.")
        .AppendLine($"Ми - команда розробників бота {StringMetadata.Team}.")
        .AppendLine($"Ви - користувач бота {StringMetadata.User}.")
        .AppendLine()
        .AppendLine("Ми збираємо мінімум інформації про вас, щоб забезпечити роботу бота:")
        .AppendLine("- ID користувача: для ідентифікації гаманця.")
        .AppendLine("- Інформація про гаманець: для виконання операцій з ним.")
        .AppendLine("- Логи повідомлень: тільки тих, які починаються з '/'.")
        .AppendLine("- Логи дій: тільки ті, які ви здійснюєте.")
        .AppendLine()
        .AppendLine("Навіщо ми це робимо:")
        .AppendLine("- Для забезпечення роботи бота.")
        .AppendLine()
        .AppendLine("Ваші дані в безпеці:")
        .AppendLine("- Ми не передаємо дані третім особам.")
        .AppendLine("- Ми не використовуємо дані в комерційних цілях.")
        .AppendLine("- Ми не зберігаємо дані довше, ніж це необхідно.")
        .AppendLine()
        .AppendLine("Ваші права:")
        .AppendLine("- Ви можете запросити видалення даних.")
        .AppendLine("- Ви можете запросити вивантаження даних.")
        .AppendLine()
        .AppendLine("Зміни в політиці:")
        .AppendLine("- Ми можемо вносити зміни в політику конфіденційності в будь-який момент.")
        .AppendLine()
        .AppendLine("Зв'язок з нами:")
        .AppendLine("- По всіх питаннях пишіть на пошту: fembina@pm.me.")
        .AppendLine()
        .AppendLine("Згода:")
        .AppendLine("- Використовуючи бота, ви погоджуєтесь з нашою політикою конфіденційності.");
}

using System.Text;

namespace Falko.Coin.Bot.Localizations;

public sealed class RussianLocalization : ILocalization
{
    public StringWithMetadataBuilder WalletIsMissing =>
        $"У {StringMetadata.User} нет кошелька";

    public StringWithMetadataBuilder WalletIsExists =>
        $"У {StringMetadata.User} уже есть кошелек";

    public StringWithMetadataBuilder WalletIsCreated =>
        $"Создан кошелек для {StringMetadata.User} с адресом {StringMetadata.Address}";

    public StringWithMetadataBuilder WalletBalance =>
        $"У {StringMetadata.User} на кошельке {StringMetadata.Amount} {StringMetadata.Coin}";

    public StringWithMetadataBuilder WalletAddress =>
        $"Адрес кошелька {StringMetadata.User} - {StringMetadata.Address}";

    public StringWithMetadataBuilder WalletBalanceTooLow =>
        $"У {StringMetadata.User} на кошельке недостаточно {StringMetadata.Coin}";

    public StringWithMetadataBuilder RequiredAmountAndWalletAddress =>
        "Требуется указать сумму и адрес кошелька";

    public StringWithMetadataBuilder AmountIsIncorrect =>
        $"Формат суммы '{StringMetadata.Amount}' неверный";

    public StringWithMetadataBuilder WalletAddressIsMissing =>
        $"Кошелька с адресом {StringMetadata.Address} не существует";

    public StringWithMetadataBuilder WalletAddressIsInvalid =>
        $"Формат кошелька с адресом '{StringMetadata.Address}' неверный";

    public StringWithMetadataBuilder TransactionProcessing =>
        $"Транзакция пользователя {StringMetadata.User} обрабатывается";

    public StringWithMetadataBuilder TransactionProcessed =>
        $"Пользователь {StringMetadata.User} перевел {StringMetadata.Amount} {StringMetadata.Coin} на кошелек {StringMetadata.Address}";

    public StringWithMetadataBuilder TransactionReceived =>
        $"Вы получили {StringMetadata.Amount} {StringMetadata.Coin} от кошелька {StringMetadata.Address}";

    public StringWithMetadataBuilder TransactionSent =>
        $"Вы отправили {StringMetadata.Amount} {StringMetadata.Coin} на кошелек {StringMetadata.Address}";

    public StringWithMetadataBuilder BotPrivacyPolicy => new StringBuilder()
        .AppendLine("Политика конфиденциальности")
        .AppendLine()
        .AppendLine($"Бот - {StringMetadata.Bot} бот.")
        .AppendLine($"Мы - команда разработчиков бота {StringMetadata.Team}.")
        .AppendLine($"Вы - пользователь бота {StringMetadata.User}.")
        .AppendLine()
        .AppendLine("Мы собираем минимум информации о вас, чтобы обеспечить работу бота:")
        .AppendLine("- Id пользователя: для идентификации кошелька.")
        .AppendLine("- Информация о кошельке: для выполнения операций с ним.")
        .AppendLine("- Логи сообщений: только тех, которые начинаются с '/'.")
        .AppendLine("- Логи действий: только те, которые вы совершаете.")
        .AppendLine()
        .AppendLine("Зачем мы это делаем:")
        .AppendLine("- Для обеспечения работы бота.")
        .AppendLine()
        .AppendLine("Ваши данные в безопасности:")
        .AppendLine("- Мы не передаем данные третьим лицам.")
        .AppendLine("- Мы не используем данные в коммерческих целях.")
        .AppendLine("- Мы не храним данные дольше, чем это необходимо.")
        .AppendLine()
        .AppendLine("Ваши права:")
        .AppendLine("- Вы можете запросить удаление данных.")
        .AppendLine("- Вы можете запросить выгрузку данных.")
        .AppendLine()
        .AppendLine("Изменения в политике:")
        .AppendLine("- Мы можем вносить изменения в политику конфиденциальности в любой момент.")
        .AppendLine()
        .AppendLine("Связь с нами:")
        .AppendLine("- По всем вопросам пишите на почту: fembina@pm.me.")
        .AppendLine()
        .AppendLine("Согласие:")
        .AppendLine("- Используя бота, вы соглашаетесь с нашей политикой конфиденциальности.");
}

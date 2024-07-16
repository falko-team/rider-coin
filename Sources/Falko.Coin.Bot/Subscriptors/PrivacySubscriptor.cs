using Falko.Coin.Bot.Extensions;
using Talkie.Builders;
using Talkie.Concurrent;
using Talkie.Flows;
using Talkie.Handlers;
using Talkie.Models.Messages;
using Talkie.Pipelines;
using Talkie.Signals;

namespace Falko.Coin.Bot.Subscriptors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class PrivacySubscriptor(ILogger<PrivacySubscriptor> logger) : ISubscriptor
{
    private readonly IMessage _cachedPrivacyMessage = new OutgoingMessageBuilder()
        .AddTextLine("Собираем все данные, продаем Яндексу, ФСБ и Гуглу, ФБР, а также продаем ваши данные в темных уголках интернета.")
        .AddTextLine()
        .AddTextLine("Пользуйтесь на здоровье!")
        .Build();

    public void Subscribe(ISignalFlow flow)
    {
        flow.Subscribe<IncomingMessageSignal>(signals => signals
            .SkipOlderThan(TimeSpan.FromSeconds(30))
            .OnlyCommand("privacy", logger)
            .HandleAsync((context, cancellationToken) => context
                .ToMessageController()
                .PublishMessageAsync(_cachedPrivacyMessage, cancellationToken)
                .AsValueTask()));
    }
}

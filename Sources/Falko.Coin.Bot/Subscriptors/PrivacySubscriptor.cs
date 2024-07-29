using Falko.Coin.Bot.Extensions;
using Talkie.Concurrent;
using Talkie.Controllers;
using Talkie.Flows;
using Talkie.Handlers;
using Talkie.Pipelines.Handling;
using Talkie.Signals;

namespace Falko.Coin.Bot.Subscriptors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class PrivacySubscriptor(ILogger<PrivacySubscriptor> logger) : ISubscriptor
{
    public void Subscribe(ISignalFlow flow)
    {
        flow.Subscribe<IncomingMessageSignal>(signals => signals
            .SkipOlderThan(TimeSpan.FromSeconds(30))
            .SelectOnlyCommand("privacy", logger)
            .HandleAsync((context, cancellationToken) => context
                .ToMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .BotPrivacyPolicy
                    .WithSenderProfileUser(context)
                    .WithEnvironmentProfileBot(context), cancellationToken)
                .AsValueTask()));
    }
}

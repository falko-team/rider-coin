using Falko.Coin.Bot.Extensions;
using Microsoft.Extensions.Logging;
using Talkie.Concurrent;
using Talkie.Controllers.MessageControllers;
using Talkie.Disposables;
using Talkie.Flows;
using Talkie.Handlers;
using Talkie.Pipelines.Handling;
using Talkie.Pipelines.Intercepting;
using Talkie.Signals;
using Talkie.Subscribers;

namespace Falko.Coin.Bot.Behaviors;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class PrivacySubscriber(ILogger<PrivacySubscriber> logger) : IBehaviorsSubscriber
{
    public void Subscribe(ISignalFlow flow, IRegisterOnlyDisposableScope disposables, CancellationToken cancellationToken)
    {
        flow.Subscribe<MessagePublishedSignal>(signals => signals
            .SkipSelfPublished()
            .SkipOlderThan(TimeSpan.FromSeconds(30))
            .SelectOnlyCommand("privacy", logger)
            .HandleAsync((context, cancellation) => context
                .ToMessageController()
                .PublishMessageAsync(context
                    .GetLocalization()
                    .BotPrivacyPolicy
                    .WithUserPublisherProfile(context)
                    .WithBotEnvironmentProfile(context), cancellation)
                .AsValueTask()))
            .UnsubscribeWith(disposables);
    }
}

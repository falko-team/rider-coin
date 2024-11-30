using Falko.Coin.Bot.Configurations;
using Microsoft.Extensions.Configuration;
using Talkie.Disposables;
using Talkie.Flows;
using Talkie.Subscribers;

namespace Falko.Coin.Bot.Integrations;

public sealed class TelegramSubscriber(IConfiguration configuration) : IIntegrationsSubscriber
{
    public async Task SubscribeAsync(ISignalFlow flow, IRegisterOnlyDisposableScope disposables, CancellationToken cancellationToken)
    {
        await flow.ConnectTelegramAsync(BotConfiguration.GetTelegramBotToken(), cancellationToken)
            .DisposeAsyncWith(disposables);
    }
}

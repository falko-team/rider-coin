using Falko.Coin.Bot.Subscriptors;
using Talkie.Flows;

namespace Falko.Coin.Bot.Extensions;

public static class SubscriptorsExtensions
{
    public static void Subscribe<T>(this ISignalFlow flow, IServiceProvider provider)
        where T : class, ISubscriptor
    {
        provider.GetRequiredService<T>().Subscribe(flow);
    }

    public static void AddSubscriptor<T>(this IServiceCollection collection) where T : class, ISubscriptor
    {
#pragma warning disable IL2091
        collection.AddTransient<T>();
#pragma warning restore IL2091
    }
}

using Talkie.Flows;

namespace Falko.Coin.Bot.Subscriptors;

public interface ISubscriptor
{
    void Subscribe(ISignalFlow flow);
}

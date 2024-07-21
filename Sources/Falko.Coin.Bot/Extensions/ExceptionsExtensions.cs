using Talkie.Flows;
using Talkie.Pipelines.Intercepting;
using Talkie.Signals;

namespace Falko.Coin.Bot.Extensions;

public static class ExceptionsExtensions
{
    public static Task<Exception> TakeUnobservedExceptionAsync(this ISignalFlow flow,
        CancellationToken cancellationToken = default)
    {
        return flow.TakeAsync(signals => signals
            .Only<UnobservedConnectionExceptionSignal, UnobservedPublishingExceptionSignal>(), cancellationToken)
            .ContinueWith(signal => signal.Result switch
            {
                UnobservedPublishingExceptionSignal publishingExceptionSignal => publishingExceptionSignal.Exception,
                UnobservedConnectionExceptionSignal connectionExceptionSignal => connectionExceptionSignal.Exception,
                _ => new InvalidOperationException("Unknown unobserved exception signal")
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion);
    }
}

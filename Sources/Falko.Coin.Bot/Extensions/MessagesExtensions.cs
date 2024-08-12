using Falko.Coin.Bot.Localizations;
using Talkie.Handlers;
using Talkie.Localizations;
using Talkie.Models.Messages;
using Talkie.Models.Profiles;
using Talkie.Pipelines.Intercepting;
using Talkie.Signals;
using Talkie.Validations;

namespace Falko.Coin.Bot.Extensions;

public static class MessagesExtensions
{
    public static bool TryGetWalletIdentifier(this ISignalContext<IncomingMessageSignal> context, out long walletIdentifier,
        ILogger? logger = null)
    {
        var result = context.Signal.Message.SenderProfile.Identifier.TryGetValue(out walletIdentifier);

        if (result is false)
        {
            logger?.LogWarning($"User with {context.Signal.Message.SenderProfile.Identifier} has no wallet identifier");
        }

        return result;
    }

    public static ISignalInterceptingPipelineBuilder<IncomingMessageSignal> SelectOnlyCommand(this ISignalInterceptingPipelineBuilder<IncomingMessageSignal> builder,
        string name,
        ILogger? logger = null)
    {
        name.ThrowIf().NullOrWhiteSpace();

        var pipeline = builder.Intercept(signal =>
        {
            var messageText = signal.Message.Text?.TrimStart().ToLowerInvariant();

            var commandName = $"/{name.Trim().ToLowerInvariant()}";

            if (messageText is null || messageText.Length < commandName.Length)
            {
                return false;
            }

            if (messageText.Length == commandName.Length)
            {
                return messageText == commandName
                    ? signal.Message.Mutate(message => message
                            .MutateText(_ => null))
                        .ToSignal()
                    : false;
            }

            if (messageText[commandName.Length] == ' ' && messageText.StartsWith(commandName))
            {
                return signal.Message.Mutate(message => message
                        .MutateText(text => text
                            ?.TrimStart()
                            .Substring(commandName.Length)
                            .TrimStart()))
                    .ToSignal();
            }

            if ((signal.Message.ReceiverProfile as IBotProfile)?.NickName is not { } nickName)
            {
                return false;
            }

            var botName = $"@{nickName.Trim().ToLowerInvariant()}";

            var commandBotName = $"{commandName}{botName}";

            if (messageText.Length < commandBotName.Length)
            {
                return false;
            }

            if (messageText.Length == commandBotName.Length)
            {
                return messageText == commandBotName
                    ? signal.Message.Mutate(message => message
                            .MutateText(_ => null))
                        .ToSignal()
                    : false;
            }

            return messageText[commandBotName.Length] == ' ' && messageText.StartsWith(commandBotName)
                ? signal.Message.Mutate(message => message
                        .MutateText(text => text
                            ?.TrimStart()
                            .Substring(commandBotName.Length)
                            .TrimStart()))
                    .ToSignal()
                : false;
        });

        if (logger is not null)
        {
            pipeline = pipeline.Do(signal => logger
                .LogDebug($"User with {signal.Message.SenderProfile.Identifier} invoked command /{name}"));
        }

        return pipeline;
    }

    public static IReadOnlyList<string> GetCommandArguments(this ISignalContext<IncomingMessageSignal> context)
    {
        const char space = ' ';

        return context
            .Signal
            .Message
            .Text
            ?.Trim()
            .Split(space)
            .Where(text => text.Length > 0)
            .ToArray() ?? [];
    }

    public static ILocalization GetLocalization(this ISignalContext<IncomingMessageSignal> context)
    {
        return context.Signal.Message.SenderProfile.Language switch
        {
            Language.English => LocalizationProvider.English,
            Language.Russian or Language.Belarusian => LocalizationProvider.Russian,
            Language.Ukrainian => LocalizationProvider.Ukrainian,
            _ => LocalizationProvider.Default
        };
    }
}

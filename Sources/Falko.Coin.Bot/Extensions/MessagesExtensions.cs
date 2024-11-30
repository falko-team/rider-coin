using Falko.Coin.Bot.Localizations;
using Microsoft.Extensions.Logging;
using Talkie.Handlers;
using Talkie.Localizations;
using Talkie.Models.Messages;
using Talkie.Models.Messages.Contents;
using Talkie.Models.Profiles;
using Talkie.Pipelines.Intercepting;
using Talkie.Signals;

namespace Falko.Coin.Bot.Extensions;

public static class MessagesExtensions
{
    public static bool TryGetWalletIdentifier(this ISignalContext<MessagePublishedSignal> context, out long walletIdentifier,
        ILogger? logger = null)
    {
        var result = context.Signal.Message.PublisherProfile.Identifier.TryGetValue(out walletIdentifier);

        if (result is false)
        {
            logger?.LogWarning
            (
                "User with {Publisher} has no wallet",
                context.Signal.Message.PublisherProfile.Identifier
            );
        }

        return result;
    }

    public static ISignalInterceptingPipelineBuilder<MessagePublishedSignal> SelectOnlyCommand(this ISignalInterceptingPipelineBuilder<MessagePublishedSignal> builder,
        string name,
        ILogger? logger = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        var pipeline = builder.Intercept(signal =>
        {
            var messageText = signal.Message.GetText().TrimStart().ToLowerInvariant();

            if (messageText.Length is 0) return false;

            var commandName = $"/{name.Trim().ToLowerInvariant()}";

            if (messageText.Length < commandName.Length)
            {
                return false;
            }

            if (messageText.Length == commandName.Length)
            {
                return messageText == commandName
                    ? signal.MutateMessage(message => message
                        .MutateContent(_ => MessageContent.Empty))
                    : false;
            }

            if (messageText[commandName.Length] == ' ' && messageText.StartsWith(commandName))
            {
                return signal.MutateMessage(message => message
                    .MutateContent(content => content
                        .Text
                        .TrimStart()
                        .Substring(commandName.Length)
                        .TrimStart()));
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
                    ? signal.MutateMessage(message => message
                        .MutateContent(_ => MessageContent.Empty))
                    : false;
            }

            return messageText[commandBotName.Length] == ' ' && messageText.StartsWith(commandBotName)
                ? signal.MutateMessage(message => message
                    .MutateContent(content => content
                        .Text
                        .TrimStart()
                        .Substring(commandBotName.Length)
                        .TrimStart()))
                : false;
        });

        if (logger is not null)
        {
            pipeline = pipeline.Do(signal => logger.LogDebug
            (
                "User with {Publisher} invoked command {Command}",
                signal.Message.PublisherProfile.Identifier,
                name
            ));
        }

        return pipeline;
    }

    public static IReadOnlyList<string> GetCommandArguments(this ISignalContext<MessagePublishedSignal> context)
    {
        const char space = ' ';

        var content = context
            .Signal
            .Message
            .Content;

        if (content.IsEmpty) return [];

        return content
            .Text
            .Trim()
            .Split(space)
            .Where(text => text.Length > 0)
            .ToArray();
    }

    public static ILocalization GetLocalization(this ISignalContext<MessagePublishedSignal> context)
    {
        return context.Signal.Message.PublisherProfile.Language switch
        {
            Language.English => LocalizationProvider.English,
            Language.Russian or Language.Belarusian => LocalizationProvider.Russian,
            Language.Ukrainian => LocalizationProvider.Ukrainian,
            _ => LocalizationProvider.Default
        };
    }
}

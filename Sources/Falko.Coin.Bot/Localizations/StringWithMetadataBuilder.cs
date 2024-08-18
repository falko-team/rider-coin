using System.Text;
using Talkie.Models.Messages.Contents;

namespace Falko.Coin.Bot.Localizations;

public sealed class StringWithMetadataBuilder(StringBuilder builder)
{
    public StringWithMetadataBuilder(string text) : this(new StringBuilder(text)) { }

    public StringWithMetadataBuilder WithBot(string bot) => WithMetadata(nameof(bot), bot);

    public StringWithMetadataBuilder WithUser(string user) => WithMetadata(nameof(user), user);

    public StringWithMetadataBuilder WithAmount(long amount) => WithAmount(amount.ToString());

    public StringWithMetadataBuilder WithAmount(string amount) => WithMetadata(nameof(amount), amount);

    public StringWithMetadataBuilder WithAddress(long address) => WithAddress(address.ToString());

    public StringWithMetadataBuilder WithAddress(string address) => WithMetadata(nameof(address), address);

    public string Build() => builder.ToString();

    private StringWithMetadataBuilder WithMetadata(string name, string value)
    {
        builder = builder.Replace($"@{name}", value);
        return this;
    }

    public static implicit operator string(StringWithMetadataBuilder builder) => builder.Build();

    public static implicit operator MessageContent(StringWithMetadataBuilder builder) => builder.Build();

    public static implicit operator StringWithMetadataBuilder(StringBuilder builder) => new(builder);

    public static implicit operator StringWithMetadataBuilder(string text) => new(text);
}

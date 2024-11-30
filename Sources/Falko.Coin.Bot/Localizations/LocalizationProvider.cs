using System.Collections.Specialized;

namespace Falko.Coin.Bot.Localizations;

public static class LocalizationProvider
{
    private static readonly ListDictionary Localizations = new();

    static LocalizationProvider()
    {
        RegisterLocalization<EnglishLocalization>(nameof(English));
        RegisterLocalization<RussianLocalization>(nameof(Russian));
        RegisterLocalization<UkrainianLocalization>(nameof(Ukrainian));
    }

    public static ILocalization Default => English;

    public static ILocalization English => GetLocalizationByLanguageName(nameof(English));

    public static ILocalization Russian => GetLocalizationByLanguageName(nameof(Russian));

    public static ILocalization Ukrainian => GetLocalizationByLanguageName(nameof(Ukrainian));

    public static ILocalization GetLocalizationByLanguageName(string languageName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(languageName, nameof(languageName));

        return ((Lazy<ILocalization>)Localizations[languageName.ToLowerInvariant()]!).Value;
    }

    private static void RegisterLocalization<T>(string name) where T : ILocalization, new()
    {
        Localizations.Add(name.ToLowerInvariant(), new Lazy<ILocalization>(() => new T()));
    }
}

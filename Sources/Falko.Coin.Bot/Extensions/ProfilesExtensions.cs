using Talkie.Models.Profiles;

namespace Falko.Coin.Bot.Extensions;

public static class ProfilesExtensions
{
    public static string GetDisplayName(this IProfile profile)
    {
        return profile switch
        {
            IUserProfile { NickName: not null } user => $"@_{user.NickName}",
            IUserProfile { FirstName: not null } user => user.FirstName,

            IChatProfile { NickName: not null } chat => $"@_{chat.NickName}",
            IChatProfile { Title: not null } chat => chat.Title,

            _ => profile.Identifier.ToString()
        };
    }
}

using System.Diagnostics.CodeAnalysis;
using Falko.Coin.Wallets.Wallets;

namespace Falko.Coin.Wallets.Services;

public interface IWalletsPool
{
    bool TryGet(long identifier, [MaybeNullWhen(false)] out IWallet wallet);

    bool TryAdd(long identifier, [MaybeNullWhen(false)] out IWallet wallet);
}

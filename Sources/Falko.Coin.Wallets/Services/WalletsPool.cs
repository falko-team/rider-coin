using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Falko.Coin.Wallets.Storages;
using Falko.Coin.Wallets.Wallets;

namespace Falko.Coin.Wallets.Services;

public sealed class WalletsPool(IWalletsStorage walletsStorage) : IWalletsPool
{
    private readonly object _locker = new();

    private readonly ConcurrentDictionary<long, PooledWallet> _wallets = new();

    public bool TryGet(long identifier, [MaybeNullWhen(false)] out IWallet wallet)
    {
        if (_wallets.TryGetValue(identifier, out var pooledWallet))
        {
            wallet = pooledWallet;
            return true;
        }

        if (walletsStorage.TryRead(identifier) is not { } storedWallet)
        {
            wallet = null;
            return false;
        }

        pooledWallet = new PooledWallet(walletsStorage, storedWallet);

        wallet = _wallets.GetOrAdd(identifier, pooledWallet);
        return true;
    }

    public bool TryAdd(long identifier, [MaybeNullWhen(false)] out IWallet wallet)
    {
        if (_wallets.TryGetValue(identifier, out _))
        {
            wallet = null;
            return false;
        }

        lock (_locker)
        {
            if (walletsStorage.TryRead(identifier) is not null)
            {
                wallet = null;
                return false;
            }

            var pooledWallet = new PooledWallet(walletsStorage, new Wallet(identifier));

            walletsStorage.Write(pooledWallet);

            wallet = _wallets.GetOrAdd(identifier, pooledWallet);
            return true;
        }
    }
}

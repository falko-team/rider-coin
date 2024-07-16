using Falko.Coin.Wallets.Storages;
using Falko.Coin.Wallets.Wallets;

namespace Falko.Coin.Wallets.Services;

public sealed class PooledWallet(IWalletsStorage wallets, IWallet wallet) : IWallet
{
    public long Identifier => wallet.Identifier;

    public long Balance => wallet.Balance;

    public void Deposit(long amount)
    {
        wallet.Deposit(amount);
        Sync();
    }

    public bool TryWithdraw(long amount)
    {
        if (wallet.TryWithdraw(amount) is false) return false;

        Sync();
        return true;
    }

    private void Sync()
    {
        wallets.Write(wallet);
    }
}

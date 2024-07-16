using Falko.Coin.Wallets.Wallets;

namespace Falko.Coin.Wallets.Storages;

public interface IWalletsStorage
{
    public void Write(IWallet wallet);

    public IWallet? TryRead(long identifier);

    public IEnumerable<IWallet> ReadAll();
}

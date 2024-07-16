namespace Falko.Coin.Wallets.Wallets;

public interface IWallet
{
    long Identifier { get; }

    long Balance { get; }

    void Deposit(long amount);

    bool TryWithdraw(long amount);
}

using Falko.Coin.Wallets.Wallets;

namespace Falko.Coin.Wallets.Transactions;

public static class WalletTransaction
{
    public static bool TryTransfer(IWallet fromWallet, IWallet toWallet, long amount)
    {
        if (fromWallet.TryWithdraw(amount) is false) return false;

        toWallet.Deposit(amount);
        return true;
    }
}

namespace Falko.Coin.Wallets.Wallets;

public sealed class Wallet : IWallet
{
    private long _balance;

    // ReSharper disable once ConvertToPrimaryConstructor
    public Wallet(long identifier, long balance = 0)
    {
        Identifier = identifier;
        _balance = balance;
    }

    public long Identifier { get; }

    public long Balance => _balance;

    public void Deposit(long amount)
    {
        ThrowIfAmountInvalid(amount);

        Interlocked.Add(ref _balance, amount);
    }

    public bool TryWithdraw(long amount)
    {
        ThrowIfAmountInvalid(amount);

        long balance;

        do
        {
            balance = Interlocked.Read(ref _balance);

            if (balance < amount) return false;
        } while (Interlocked.CompareExchange(ref _balance, balance - amount, balance) != balance);

        return true;
    }

    private static void ThrowIfAmountInvalid(long amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive");
        }
    }
}

using Falko.Coin.Wallets.Wallets;
using Talkie.Validations;

namespace Falko.Coin.Wallets.Storages;

public sealed class WalletsStorage : IWalletsStorage
{
    private readonly object _locker = new();

    private readonly string _directory;

    public WalletsStorage(string directory)
    {
        CreateDirectoryIfNotExists(directory);

        _directory = directory;
    }

    public void Write(IWallet wallet)
    {
        wallet.ThrowIf().Null();

        var file = GetWalletPath(wallet.Identifier);

        WriteWalletBalance(file, wallet.Balance);
    }

    public IWallet? TryRead(long identifier)
    {
        var file = GetWalletPath(identifier);

        try
        {
            var balance = ReadWalletBalance(file);

            return new Wallet(identifier, balance);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    public IEnumerable<IWallet> ReadAll()
    {
        return GetWalletPaths().Select(file =>
        {
            var identifier = GetWalletIdentifierFromWalletPath(file);

            var balance = ReadWalletBalance(file);

            return new Wallet(identifier, balance);
        });
    }

    private void WriteWalletBalance(string file, long balance)
    {
        var balanceBytes = BitConverter.GetBytes(balance);

        lock (_locker) File.WriteAllBytes(file, balanceBytes);
    }

    private long ReadWalletBalance(string file)
    {
        byte[] balance;

        lock (_locker) balance = File.ReadAllBytes(file);

        return BitConverter.ToInt64(balance);
    }

    private string GetWalletPath(long identifier)
    {
        return Path.Combine(_directory, $"{identifier}.wallet");
    }

    private IEnumerable<string> GetWalletPaths()
    {
        return Directory.EnumerateFiles(_directory, "*.wallet");
    }

    private static long GetWalletIdentifierFromWalletPath(string file)
    {
        return long.Parse(Path.GetFileNameWithoutExtension(file));
    }

    private static void CreateDirectoryIfNotExists(string directory)
    {
        if (Directory.Exists(directory) is false)
        {
            Directory.CreateDirectory(directory);
        }
    }
}

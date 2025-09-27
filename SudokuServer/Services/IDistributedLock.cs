namespace SudokuServer.Services;

public interface IDistributedLock
{
    Task<IDistributedLockObject> LockAsync(string key, TimeSpan timeout);
}

public interface IDistributedLockObject : IAsyncDisposable
{
    string Key { get; }

    string Value { get; }

    bool IsLocked { get; }

    Task<bool> UnlockAsync();
}

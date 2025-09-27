using StackExchange.Redis;
using SudokuServer.Services;

namespace SudokuServer.ServicesImpl;

public class RedisDistributedLock(IDatabase redis) : IDistributedLock
{
    public async Task<IDistributedLockObject> LockAsync(string key, TimeSpan timeout)
    {
        string value = Guid.NewGuid().ToString();
        bool isLocked = await redis.LockTakeAsync(key, value, timeout);
        return new RedisDistributedLockObject(redis, key, value, isLocked);
    }
}

internal class RedisDistributedLockObject(IDatabase redis, string key, string value, bool isLocked)
    : IDistributedLockObject
{
    public string Key => key;

    public string Value => value;

    public bool IsLocked { get; private set; } = isLocked;

    public async ValueTask DisposeAsync()
    {
        await UnlockAsync();
    }

    public async Task<bool> UnlockAsync()
    {
        if (!IsLocked)
            return false;
        bool isRelease = await redis.LockReleaseAsync(Key, Value);
        if (isRelease)
        {
            IsLocked = false;
            return true;
        }
        return false;
    }
}

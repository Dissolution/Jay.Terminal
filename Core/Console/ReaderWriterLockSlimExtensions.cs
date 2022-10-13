namespace Jay.Terminalis.Console;

public static class ReaderWriterLockSlimExtensions
{
    public static void WaitForNoLocks(this ReaderWriterLockSlim slimLock)
    {
        while (slimLock.WaitingReadCount > 0 ||
               slimLock.WaitingWriteCount > 0 ||
               slimLock.WaitingUpgradeCount > 0 ||
               slimLock.CurrentReadCount > 0)
        {
            Thread.SpinWait(1);
        }
    }
    
    public static async Task WaitForNoLocksAsync(this ReaderWriterLockSlim slimLock, CancellationToken token = default)
    {
        while (slimLock.WaitingReadCount > 0 ||
               slimLock.WaitingWriteCount > 0 ||
               slimLock.WaitingUpgradeCount > 0 ||
               slimLock.CurrentReadCount > 0)
        {
            await Task.Delay(1, token);
        }
    }

    public static IDisposable GetReadLock(this ReaderWriterLockSlim slimLock)
    {
        return new ReadWriteLock(slimLock, true, false);
    }
    public static IDisposable GetWriteLock(this ReaderWriterLockSlim slimLock)
    {
        return new ReadWriteLock(slimLock, false, true);
    }

    private sealed class ReadWriteLock : IDisposable
    {
        private readonly ReaderWriterLockSlim _slimLock;

        public bool HasReadLock { get; private set; } = false;

        public bool HasWriteLock { get; private set; } = false;
        
        public ReadWriteLock(ReaderWriterLockSlim slimLock, bool read, bool write)
        {
            if (read == write) throw new ArgumentException();
            _slimLock = slimLock;
            if (read)
            {
                while (!slimLock.TryEnterReadLock(1))
                    Thread.SpinWait(1);
                HasReadLock = true;
            }
            else if (write)
            {
                while (!slimLock.TryEnterWriteLock(1))
                    Thread.SpinWait(1);
                HasWriteLock = true;
            }
        }

        public void Dispose()
        {
            if (HasReadLock)
            {
                _slimLock.ExitReadLock();
            }
            else if (HasWriteLock)
            {
                _slimLock.ExitWriteLock();
            }
        }
    }
}
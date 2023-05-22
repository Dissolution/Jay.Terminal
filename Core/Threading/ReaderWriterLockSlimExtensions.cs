namespace Jay.Terminalis.Threading;

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
        return ReadLock.Acquire(slimLock);
    }

    private sealed class ReadLock : IDisposable
    {
        public static ReadLock Acquire(ReaderWriterLockSlim slimLock)
        {
            while (!slimLock.TryEnterReadLock(1))
                Thread.SpinWait(1);
            return new ReadLock(slimLock);
        }
        
        private readonly ReaderWriterLockSlim _slimLock;

        private ReadLock(ReaderWriterLockSlim slimLock)
        {
            _slimLock = slimLock;
        }

        public void Dispose()
        {
            _slimLock.ExitReadLock();
        }
    }
    
    public static IDisposable GetWriteLock(this ReaderWriterLockSlim slimLock)
    {
        return WriteLock.Acquire(slimLock);
    }

    private sealed class WriteLock : IDisposable
    {
        public static WriteLock Acquire(ReaderWriterLockSlim slimLock)
        {
            while (!slimLock.TryEnterWriteLock(1))
                Thread.SpinWait(1);
            return new WriteLock(slimLock);
        }
        
        private readonly ReaderWriterLockSlim _slimLock;

        private WriteLock(ReaderWriterLockSlim slimLock)
        {
            _slimLock = slimLock;
        }

        public void Dispose()
        {
            _slimLock.ExitWriteLock();
        }
    }
}
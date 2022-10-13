global using SysCons = System.Console;

using System.Linq.Expressions;


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
}

public interface ITerminalInstance : IDisposable
{
    ITerminalInput Input { get; }
    ITerminalOutput Output { get; }
    ITerminalError Error { get; }
    
    ITerminalBuffer Buffer { get; }
}

internal abstract class TerminalInstance
{
    private readonly ReaderWriterLockSlim _slimLock;

    protected TerminalInstance(ReaderWriterLockSlim slimLock)
    {
        _slimLock = slimLock;
    }

    protected TValue GetValue<TValue>(Func<TValue> getConsoleValue)
    {
        _slimLock.TryEnterReadLock(-1);
        TValue value;
        try
        {
            value = getConsoleValue();
        }
        finally
        {
            _slimLock.ExitReadLock();
        }
        return value;
    }

    protected void SetValue<TValue>(Action<TValue> setConsoleValue, TValue value)
    {
        _slimLock.TryEnterWriteLock(-1);
        try
        {
            setConsoleValue(value);
        }
        finally
        {
            _slimLock.ExitWriteLock();
        }
    }

    protected void Interact(Action consoleAction)
    {
        _slimLock.TryEnterWriteLock(-1);
        try
        {
            consoleAction();
        }
        finally
        {
            _slimLock.ExitWriteLock();
        }
    }
}

public abstract class TerminalInstanceBase : ITerminalInstance
{
    private readonly ReaderWriterLockSlim _lock;

    protected TerminalInstanceBase()
    {
        _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    }

    public void WaitAndDispose()
    {
        _lock.WaitForNoLocks();
        Dispose();
        System.Console.
    }
    
    public virtual void Dispose()
    {
        _lock.Dispose();
    }
}
namespace Jay.Terminalis;

internal sealed class ActionDisposable : IDisposable
{
    private Action? _disposeAction;

    public ActionDisposable(Action? disposeAction)
    {
        _disposeAction = disposeAction;
    }

    public void Dispose()
    {
        _disposeAction?.Invoke();
        _disposeAction = null;
    }
}
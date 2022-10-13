using System.Linq.Expressions;
using System.Reflection;

namespace Jay.Terminalis.Console;

internal class TerminalReset : IDisposable
{
    private readonly ITerminalInstance _terminal;
    private readonly List<(PropertyInfo Property, object? Value)> _originalTerminalPropertyValues;
    
    public TerminalReset(ITerminalInstance terminal)
    {
        _terminal = terminal;
        _originalTerminalPropertyValues = new(2);
    }

    public TerminalReset Watch(Expression<Func<ITerminalInstance, object?>> propertyExpression)
    {
        if (propertyExpression is not LambdaExpression lambdaExpression)
            throw new InvalidOperationException();
        if (lambdaExpression.Body is not MemberExpression memberExpression)
            throw new InvalidOperationException();
        if (memberExpression.Member is not PropertyInfo propertyInfo)
            throw new InvalidOperationException();
        var value = propertyInfo.GetValue(_terminal);
        _originalTerminalPropertyValues.Add((propertyInfo, value));
        return this;
    }
    
    public virtual void Dispose()
    {
        // Reverse order
        for (var i = _originalTerminalPropertyValues.Count; i >= 0; i--)
        {
            (PropertyInfo? property, object? value) = _originalTerminalPropertyValues[i];
            property.SetValue(this, value);
        }
    }
    
}
using System;

namespace GameCore.Utility;

public abstract class EventFilter<T>
{
    protected EventFilter(T source)
    {
        Source = source;
    }

    public T Source { get; set; }
    public event Action? ConditionChanged;
    public abstract void SubscribeEvents();
    public abstract void UnsubscribeEvents();
    protected void RaiseConditionChanged() => ConditionChanged?.Invoke();
}

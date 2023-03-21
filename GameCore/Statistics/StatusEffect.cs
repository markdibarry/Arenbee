﻿namespace GameCore.Statistics;

public class StatusEffect
{
    public StatusEffect(AStats stats, StatusEffectData effectData, IConditionEventFilterFactory factory)
    {
        Stats = stats;
        EffectData = effectData;
        if (effectData.TickCondition != null)
        {
            _tickCondition = new(effectData.TickCondition);
            _tickCondition.EventFilter = factory.GetEventFilter(stats, _tickCondition);
        }
    }

    private readonly Condition? _tickCondition;
    public StatusEffectData EffectData { get; }
    public int EffectType => EffectData.EffectType;
    public AStats Stats { get; }

    public void CallEffectTick()
    {
        if (_tickCondition?.EventFilter == null)
            return;
        if (!_tickCondition.EventFilter.CheckCondition())
            return;
        EffectData.TickEffect?.Invoke(this);
        _tickCondition.Reset();
    }

    public void SubscribeCondition()
    {
        if (_tickCondition?.EventFilter != null)
        {
            _tickCondition.EventFilter.SubscribeEvents();
            _tickCondition.EventFilter.ConditionChanged += CallEffectTick;
        }
    }

    public void UnsubscribeCondtion()
    {
        if (_tickCondition?.EventFilter != null)
        {
            _tickCondition.EventFilter.UnsubscribeEvents();
            _tickCondition.EventFilter.ConditionChanged -= CallEffectTick;
        }
    }
}

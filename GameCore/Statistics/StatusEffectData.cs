using System;
using System.Collections.Generic;

namespace GameCore.Statistics;

public class StatusEffectData
{
    public string Name { get; init; }
    public string AbbrName { get; init; }
    public string PastTenseName { get; init; }
    public string Description { get; init; }
    public int EffectType { get; init; }
    public Action<StatusEffect>? EnterEffect { get; init; }
    public Action<StatusEffect>? ExitEffect { get; init; }
    public IReadOnlyCollection<Modifier> EffectModifiers { get; init; } = Array.Empty<Modifier>();
    public Condition? TickCondition { get; init; }
    public Action<StatusEffect>? TickEffect { get; init; }
}

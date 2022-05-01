using System;
using System.Collections.Generic;

namespace Arenbee.Framework.Statistics
{
    public enum StatusEffectType
    {
        None,
        KO,
        Burn,
        Freeze,
        Paralysis,
        Poison,
        Zombie,
        Attack
    }

    public class StatusEffectData
    {
        public string Name { get; init; }
        public string AbbrName { get; init; }
        public string PastTenseName { get; init; }
        public string Description { get; init; }
        public StatusEffectType EffectType { get; init; }
        public Action<Stats> EnterEffect { get; init; }
        public Action<Stats> ExitEffect { get; init; }
        public StatsNotifier ExpireNotifier { get; init; }
        public Func<StatusEffect, List<Modifier>> GetEffectModifiers { get; init; }
        public StatsNotifier TickNotifier { get; init; }
        public Action<StatusEffect> TickEffect { get; set; }
    }
}
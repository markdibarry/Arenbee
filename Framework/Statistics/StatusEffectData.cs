using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Statistics
{
    public abstract class StatusEffectData
    {
        protected StatusEffectType EffectType { get; set; }
        public virtual List<Modifier> GetEffectModifiers(StatusEffect statusEffect) { return new List<Modifier>(); }
        public virtual TickData GetTickData(Stats stats) { return null; }
        public virtual TempModifier GetTempModifier(Modifier modifier)
        {
            return new TimedModifier(modifier);
        }
        public virtual void ApplyEffect(Stats stats) { }
        public virtual void RemoveEffect(Stats stats) { }
    }

    public class Burn : StatusEffectData
    {
        public Burn() { EffectType = StatusEffectType.Burn; }

        public override void ApplyEffect(Stats stats)
        {
            if (stats.StatsOwner is not Actor actor)
                return;
            actor.IsRunStuck++;
        }

        public override void RemoveEffect(Stats stats)
        {
            if (stats.StatsOwner is not Actor actor)
                return;
            actor.IsRunStuck--;
        }

        public override TickData GetTickData(Stats stats)
        {
            return new TimedTick(stats, (stats) =>
            {
                if (stats.StatsOwner is Actor actor)
                {
                    var actionData = new ActionData(actor, ActionType.Status)
                    {
                        SourceName = EffectType.Get().Name,
                        StatusEffectDamage = EffectType,
                        Value = (int)(actor.Stats.Attributes.GetStat(AttributeType.MaxHP).ModifiedValue * 0.05)
                    };
                    actor.Stats.TakeDamage(actionData);
                }
            });
        }
    }

    public class Freeze : StatusEffectData
    {
        public Freeze() { EffectType = StatusEffectType.Freeze; }
    }

    public class Paralyze : StatusEffectData
    {
        public Paralyze() { EffectType = StatusEffectType.Paralysis; }
    }

    public class Poison : StatusEffectData
    {
        public Poison() { EffectType = StatusEffectType.Poison; }

        public override void ApplyEffect(Stats stats)
        {
            if (stats.StatsOwner is not Actor actor)
                return;
            actor.HalfSpeed++;
        }

        public override void RemoveEffect(Stats stats)
        {
            if (stats.StatsOwner is not Actor actor)
                return;
            actor.HalfSpeed--;
        }

        public override TickData GetTickData(Stats stats)
        {
            return new TimedTick(stats, (stats) =>
            {
                if (stats.StatsOwner is not Actor actor)
                    return;
                var actionData = new ActionData(actor, ActionType.Status)
                {
                    SourceName = EffectType.Get().Name,
                    StatusEffectDamage = EffectType,
                    Value = (int)(actor.Stats.Attributes.GetStat(AttributeType.MaxHP).ModifiedValue * 0.05)
                };
                actor.Stats.TakeDamage(actionData);
            });
        }
    }

    public class Attack : StatusEffectData
    {
        public Attack() { EffectType = StatusEffectType.Attack; }

        public override List<Modifier> GetEffectModifiers(StatusEffect effect)
        {
            return new()
            {
                new Modifier(
                    StatType.Attribute,
                    (int)AttributeType.Attack,
                    ModEffect.Percentage,
                    effect.ModifiedValue > 0 ? 120 : 80)
            };
        }
    }
}
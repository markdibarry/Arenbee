using System.Collections.Generic;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Enums;

namespace Arenbee.Framework.Statistics
{
    public class StatusEffectDB : IStatusEffectDB
    {
        public StatusEffectDB()
        {
            _effects = new Dictionary<StatusEffectType, StatusEffectData>();
            BuildDB();
        }

        private readonly Dictionary<StatusEffectType, StatusEffectData> _effects;

        public StatusEffectData GetEffectData(StatusEffectType type)
        {
            if (_effects.TryGetValue(type, out var effect))
                return effect;
            return null;
        }

        public StatusEffectData GetEffectData(int type)
        {
            return GetEffectData((StatusEffectType)type);
        }

        private void BuildDB()
        {
            _effects[StatusEffectType.KO] = new StatusEffectData()
            {
                EffectType = StatusEffectType.KO,
                Name = "KO",
                AbbrName = "KO",
                PastTenseName = "KO'd",
                Description = "Character is unable to fight!"
            };
            _effects[StatusEffectType.Burn] = new StatusEffectData()
            {
                EffectType = StatusEffectType.Burn,
                Name = "Burn",
                AbbrName = "Brn",
                PastTenseName = "Burned",
                Description = "Character takes fire damage and runs to put out the flames!",
                EnterEffect = (stats) =>
                {
                    if (stats.StatsOwner is not Actor actor)
                        return;
                    actor.IsRunStuck++;
                },
                ExitEffect = (stats) =>
                {
                    if (stats.StatsOwner is not Actor actor)
                        return;
                    actor.IsRunStuck--;
                },
                ExpireNotifier = new TimedNotifier(10f, true),
                TickNotifier = new TimedNotifier(3f, false),
                TickEffect = (statusEffect) =>
                {
                    var statsOwner = statusEffect.Stats.StatsOwner;
                    if (statsOwner is Actor actor)
                        actor.InputHandler.Jump.IsActionJustPressed = true;
                    var actionData = new ActionData()
                    {
                        Value = (int)(statsOwner.Stats.GetMaxHP() * 0.05),
                        SourceName = statusEffect.EffectData.Name,
                        ActionType = ActionType.Status,
                        StatusEffectDamage = statusEffect.StatusEffectType
                    };
                    statsOwner.Stats.ReceiveAction(actionData);
                }
            };
            _effects[StatusEffectType.Freeze] = new StatusEffectData()
            {
                EffectType = StatusEffectType.Freeze,
                Name = "Freeze",
                AbbrName = "Frz",
                PastTenseName = "Frozen",
                Description = "Character can't move.",
                ExpireNotifier = new TimedNotifier(10f, true),
            };
            _effects[StatusEffectType.Paralysis] = new StatusEffectData()
            {
                EffectType = StatusEffectType.Paralysis,
                Name = "Paralysis",
                AbbrName = "Pyz",
                PastTenseName = "Paralyzed",
                Description = "Character can't move.",
                ExpireNotifier = new TimedNotifier(10f, true),
            };
            _effects[StatusEffectType.Poison] = new StatusEffectData()
            {
                EffectType = StatusEffectType.Poison,
                Name = "Poison",
                AbbrName = "Psn",
                PastTenseName = "Poisoned",
                Description = "Feel nauseous and hurt??",
                EnterEffect = (stats) =>
                {
                    if (stats.StatsOwner is not Actor actor)
                        return;
                    actor.HalfSpeed++;
                },
                ExitEffect = (stats) =>
                {
                    if (stats.StatsOwner is not Actor actor)
                        return;
                    actor.HalfSpeed--;
                },
                ExpireNotifier = new TimedNotifier(10f, true),
                TickNotifier = new TimedNotifier(3f),
                TickEffect = (statusEffect) =>
                {
                    var statsOwner = statusEffect.Stats.StatsOwner;
                    var actionData = new ActionData()
                    {
                        Value = (int)(statsOwner.Stats.GetMaxHP() * 0.05),
                        SourceName = statusEffect.EffectData.Name,
                        ActionType = ActionType.Status,
                        StatusEffectDamage = statusEffect.StatusEffectType
                    };
                    statsOwner.Stats.ReceiveAction(actionData);
                }
            };
            _effects[StatusEffectType.Zombie] = new StatusEffectData()
            {
                EffectType = StatusEffectType.Zombie,
                Name = "Zombie",
                AbbrName = "Zom",
                PastTenseName = "Zombified",
                Description = "Character takes damage from healing.",
                ExpireNotifier = new TimedNotifier(10f, true)
            };
            _effects[StatusEffectType.Attack] = new StatusEffectData()
            {
                EffectType = StatusEffectType.Attack,
                Name = "Attack",
                AbbrName = "Atk",
                Description = "Character's Attack is increased or decreased.",
                ExpireNotifier = new TimedNotifier(10f, true),
                GetEffectModifiers = (statusEffect) =>
                {
                    return new()
                    {
                        new Modifier(
                            StatType.Attribute,
                            (int)AttributeType.Attack,
                            ModOperator.Multiply,
                            statusEffect.ModifiedValue > 0 ? 20 : -20)
                    };
                }
            };
        }
    }
}
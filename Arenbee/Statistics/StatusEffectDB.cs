using GameCore.Enums;
using GameCore.Statistics;

namespace Arenbee.Statistics;

public class StatusEffectDB : AStatusEffectDB
{
    protected override StatusEffectData[] BuildDB()
    {
        return new StatusEffectData[]
        {
            new()
            {
                EffectType = (int)StatusEffectType.KO,
                Name = "KO",
                AbbrName = "KO",
                PastTenseName = "KO'd",
                Description = "Character is unable to fight!"
            },
            new()
            {
                EffectType = (int)StatusEffectType.Burn,
                Name = "Burn",
                AbbrName = "Brn",
                PastTenseName = "Burned",
                Description = "Character takes fire damage and runs to put out the flames!",
                EffectModifiers = new Modifier[]
                {
                    new Modifier((int)StatType.Attack, ModOp.Percent, 80)
                },
                TickCondition = new()
                {
                    ConditionType = 0,
                    TargetValue = 3
                },
                TickEffect = StatusEffectMethods.BurnTick
            },
            new()
            {
                EffectType = (int)StatusEffectType.Freeze,
                Name = "Freeze",
                AbbrName = "Frz",
                PastTenseName = "Frozen",
                Description = "Character can't move.",
            },
            new()
            {
                EffectType = (int)StatusEffectType.Paralysis,
                Name = "Paralysis",
                AbbrName = "Pyz",
                PastTenseName = "Paralyzed",
                Description = "Character can't move."
            },
            new()
            {
                EffectType = (int)StatusEffectType.Poison,
                Name = "Poison",
                AbbrName = "Psn",
                PastTenseName = "Poisoned",
                Description = "Feel nauseous.",
                TickCondition = new()
                {
                    ConditionType = 0,
                    TargetValue = 3
                },
                TickEffect = StatusEffectMethods.PoisonTick
            },
            new()
            {
                EffectType = (int)StatusEffectType.Zombie,
                Name = "Zombie",
                AbbrName = "Zom",
                PastTenseName = "Zombified",
                Description = "Character takes damage from healing.",
            }
        };
    }
}

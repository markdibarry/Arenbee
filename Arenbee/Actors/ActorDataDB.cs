using System;
using Arenbee.Items;
using GameCore.Actors;
using GameCore.Statistics;

namespace Arenbee.Actors;

public class ActorDataDB : AActorDataDB
{
    protected override IActorData[] BuildDB()
    {
        return new ActorData[]
        {
            new(actorId: "Ball",
                actorName: "Ball",
                attributeData: new AttributeData[]
                {
                    new(AttributeType.MaxHP, 6),
                    new(AttributeType.HP, 6),
                    new(AttributeType.Attack, 4),
                    new(AttributeType.Defense, 0)
                },
                modifiers: new Modifier[]
                {
                    new(StatType.ElementDef, (int)ElementType.Earth, ModOperator.Add, ElementDef.Weak),
                    new(StatType.StatusEffectOff, (int)StatusEffectType.Poison, ModOperator.Add, 1, 100)
                },
                equipmentSlotData: Array.Empty<EquipmentSlotData>(),
                itemStackData: Array.Empty<ItemStackData>()),

            new(actorId: "Orc",
                actorName: "Orc",
                attributeData: new AttributeData[]
                {
                    new(AttributeType.MaxHP, 6),
                    new(AttributeType.HP, 6),
                    new(AttributeType.Attack, 4),
                    new(AttributeType.Defense, 0)
                },
                modifiers: new Modifier[]
                {
                    new(StatType.ElementDef, (int)ElementType.Earth, ModOperator.Add, ElementDef.Weak),
                    new(StatType.StatusEffectOff, (int)StatusEffectType.Poison, ModOperator.Add, 1, 100)
                },
                equipmentSlotData: Array.Empty<EquipmentSlotData>(),
                itemStackData: Array.Empty<ItemStackData>()),

            new(actorId: "Plant",
                actorName: "Plant",
                attributeData: new AttributeData[]
                {
                    new(AttributeType.MaxHP, 4),
                    new(AttributeType.HP, 4),
                    new(AttributeType.Attack, 4),
                    new(AttributeType.Defense, 0)
                },
                modifiers: new Modifier[]
                {
                    new(StatType.ElementOff, (int)ElementType.Earth, ModOperator.Add, 1),
                    new(StatType.ElementDef, (int)ElementType.Fire, ModOperator.Add, ElementDef.Weak)
                },
                equipmentSlotData: Array.Empty<EquipmentSlotData>(),
                itemStackData: Array.Empty<ItemStackData>()),

            new(actorId: "Whisp",
                actorName: "Whisp",
                attributeData: new AttributeData[]
                {
                    new(AttributeType.MaxHP, 4),
                    new(AttributeType.HP, 4),
                    new(AttributeType.Attack, 2),
                    new(AttributeType.Defense, 0)
                },
                modifiers: new Modifier[]
                {
                    new(StatType.ElementOff, (int)ElementType.Fire, ModOperator.Add, 1),
                    new(StatType.StatusEffectOff, (int)StatusEffectType.Burn, ModOperator.Add, value: 1, chance: 100),
                    new(StatType.StatusEffectDef, (int)StatusEffectType.Burn, ModOperator.Add, value: 100),
                    new(StatType.ElementDef, (int)ElementType.Water, ModOperator.Add, ElementDef.Weak),
                    new(StatType.ElementDef, (int)ElementType.Earth, ModOperator.Add, ElementDef.Resist),
                    new(StatType.ElementDef, (int)ElementType.Fire, ModOperator.Add, ElementDef.Absorb)
                },
                equipmentSlotData: Array.Empty<EquipmentSlotData>(),
                itemStackData: Array.Empty<ItemStackData>()),

            new(actorId: "Twosen",
                actorName: "Twosen",
                attributeData: new AttributeData[]
                {
                    new(AttributeType.MaxHP, 12),
                    new(AttributeType.HP, 12),
                    new(AttributeType.Attack, 0),
                    new(AttributeType.Defense, 0)
                },
                modifiers: Array.Empty<Modifier>(),
                equipmentSlotData: new EquipmentSlotData[]
                {
                    new(EquipmentSlotCategoryIds.Weapon, 0)
                },
                itemStackData: new ItemStackData[]
                {
                    new("HockeyStick", 1)
                })
        };
    }
}

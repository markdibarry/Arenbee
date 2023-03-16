using System;
using Arenbee.Items;
using Arenbee.Statistics;
using GameCore.Actors;
using GameCore.Enums;
using GameCore.Statistics;

namespace Arenbee.Actors;

public class ActorDataDB : AActorDataDB
{
    protected override AActorData[] BuildDB()
    {
        return new ActorData[]
        {
            new(actorId: "Ball",
                actorName: "Ball",
                statsData: new StatsData(
                    statLookup: new Stat[]
                    {
                        new((int)StatType.MaxHP, 6),
                        new((int)StatType.HP, 6),
                        new((int)StatType.Attack, 4),
                        new((int)StatType.Defense, 0)
                    },
                    modifiers: new Modifier[]
                    {
                        new((int)StatType.EarthResist, ModOp.Add, ElementResist.Weak),
                        new((int)StatType.PoisonAttack, ModOp.Add, 100)
                    }),
                equipmentSlotData: Array.Empty<EquipmentSlotData>(),
                itemStackData: Array.Empty<ItemStackData>()),

            new(actorId: "Orc",
                actorName: "Orc",
                statsData: new StatsData(
                    statLookup: new Stat[]
                    {
                        new((int)StatType.MaxHP, 6),
                        new((int)StatType.HP, 6),
                        new((int)StatType.Attack, 4),
                        new((int)StatType.Defense, 0)
                    },
                    modifiers: new Modifier[]
                    {
                        new((int)StatType.EarthResist, ModOp.Add, ElementResist.Weak),
                        new((int)StatType.PoisonAttack, ModOp.Add, 100)
                    }),
                equipmentSlotData: Array.Empty<EquipmentSlotData>(),
                itemStackData: Array.Empty<ItemStackData>()),

            new(actorId: "Plant",
                actorName: "Plant",
                statsData: new StatsData(
                    statLookup: new Stat[]
                    {
                        new((int)StatType.MaxHP, 4),
                        new((int)StatType.HP, 4),
                        new((int)StatType.Attack, 4),
                        new((int)StatType.Defense, 0)
                    },
                    modifiers: new Modifier[]
                    {
                        new((int)StatType.AttackElement, ModOp.Add, (int)ElementType.Earth),
                        new((int)StatType.FireResist, ModOp.Add, ElementResist.Weak)
                    }),
                equipmentSlotData: Array.Empty<EquipmentSlotData>(),
                itemStackData: Array.Empty<ItemStackData>()),

            new(actorId: "Whisp",
                actorName: "Whisp",
                statsData: new StatsData(
                    statLookup: new Stat[]
                    {
                        new((int)StatType.MaxHP, 4),
                        new((int)StatType.HP, 4),
                        new((int)StatType.Attack, 2),
                        new((int)StatType.Defense, 0)
                    },
                    modifiers: new Modifier[]
                    {
                        new((int)StatType.AttackElement, ModOp.Add, (int)ElementType.Fire),
                        new((int)StatType.BurnAttack, ModOp.Add, 100),
                        new((int)StatType.BurnResist, ModOp.Add, 100),
                        new((int)StatType.WaterResist, ModOp.Add, ElementResist.Weak),
                        new((int)StatType.EarthResist, ModOp.Add, ElementResist.Resist),
                        new((int)StatType.FireResist, ModOp.Add, ElementResist.Absorb)
                    }),
                equipmentSlotData: Array.Empty<EquipmentSlotData>(),
                itemStackData: Array.Empty<ItemStackData>()),

            new(actorId: "Twosen",
                actorName: "Twosen",
                statsData: new StatsData(
                    statLookup: new Stat[]
                    {
                        new((int)StatType.MaxHP, 12),
                        new((int)StatType.HP, 12),
                        new((int)StatType.Attack, 0),
                        new((int)StatType.Defense, 0)
                    },
                    modifiers: Array.Empty<Modifier>()),
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

using System.Collections.Generic;
using GameCore.Actors;
using GameCore.Statistics;
using GameCore.Items;
using GameCore.Extensions;

namespace GameCore.SaveData;

public class ActorData
{
    public ActorData()
    { }

    public ActorData(ActorBase actor)
    {
        ActorPath = actor.SceneFilePath;
        EquipmentSlots = actor.Equipment.Slots;
        Stats = actor.Stats;
    }

    public string ActorPath { get; set; }
    public IEnumerable<EquipmentSlotBase> EquipmentSlots { get; set; }
    public Stats Stats { get; set; }

    public ActorBase GetActor(Inventory inventory)
    {
        var actor = GDEx.Instantiate<ActorBase>(ActorPath);
        actor.Stats.ApplyStats(Stats);
        actor.Inventory = inventory;
        actor.Equipment.ApplyEquipment(EquipmentSlots);
        return actor;
    }
}

using System.Collections.Generic;
using GameCore.Items;
using Godot;

namespace GameCore.Actors;

public abstract partial class AActorData : Resource
{
    public string ActorId { get; set; }

    public abstract AActorData Clone();
    public abstract AActor CreateActor(AItemDB itemDB, IEnumerable<EquipmentSlotCategory> equipmentSlotCategories, AInventory? externalInventory);
}

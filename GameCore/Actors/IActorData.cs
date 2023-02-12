using System.Collections.Generic;
using GameCore.Items;

namespace GameCore.Actors;

public interface IActorData
{
    string ActorId { get; }

    IActorData Clone();
    AActor CreateActor(AItemDB itemDB, IEnumerable<EquipmentSlotCategory> equipmentSlotCategories);
}

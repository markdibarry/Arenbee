using GameCore.Items;
using Godot;

namespace GameCore.Actors;

public abstract partial class AActorData : Resource
{
    public string ActorId { get; set; } = string.Empty;

    public abstract AActorData Clone();
    public abstract AActor CreateActor(string equipmentSlotPresetId, AInventory? externalInventory);
}

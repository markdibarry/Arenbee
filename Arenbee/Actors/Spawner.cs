using GameCore.Actors;
using Godot;

namespace Arenbee.Actors;

[Tool]
public partial class Spawner : ASpawner
{
    public override AActorBody? Spawn()
    {
        if (ActorData == null && ActorDataId != string.Empty)
            ActorData = ActorDataDB.GetData<ActorData>(ActorDataId)?.Clone();
        if (ActorData == null || ActorBody == null)
            return null;
        AActor actor = ((ActorData)ActorData).CreateActor();
        AActorBody actorBody = (AActorBody)ActorBody.Duplicate();
        actor.SetActorBody(actorBody);
        actorBody.SetActor(actor);
        actorBody.GlobalPosition = GlobalPosition;
        SpawnPending = false;
        return actorBody;
    }
}

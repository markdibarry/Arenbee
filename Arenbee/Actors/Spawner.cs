using GameCore.Actors;
using Godot;

namespace Arenbee.Actors;

[Tool]
public partial class Spawner : ASpawner
{
    public override void Spawn()
    {
        if (Engine.IsEditorHint())
            return;
        if (ActorData == null || ActorBody == null)
            return;
        AActor actor = ((ActorData)ActorData).CreateActor();
        AActorBody actorBody = (AActorBody)ActorBody.Duplicate();
        actorBody.Actor = actor;
        actor.ActorBody = actorBody;
        actorBody.GlobalPosition = GlobalPosition;
        Owner.CallDeferred(MethodName.AddChild, actorBody);
    }
}

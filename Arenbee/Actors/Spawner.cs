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
        actor.SetActorBody(actorBody);
        actorBody.SetActor(actor);
        actorBody.GlobalPosition = GlobalPosition;
        GetParent().CallDeferred(Node.MethodName.AddChild, actorBody);
    }
}

using GameCore.Actors;
using Godot;

namespace Arenbee.Actors;

[Tool]
public partial class Spawner : BaseSpawner
{
    protected override int DefaultActorRole => (int)ActorRole.Enemy;
}

using System.Collections.Generic;
using System.Linq;
using Arenbee.Items;
using GameCore.Actors;
using GameCore.Items;

namespace Arenbee.Game;

public class Party
{
    public Party(string id)
    {
        Id = id;
        Inventory = new Inventory();
        _actors = new();
    }

    public Party(string id, IEnumerable<AActor> actors, AInventory inventory)
    {
        Id = id;
        Inventory = inventory;
        _actors = actors.ToList();
    }

    private readonly List<AActor> _actors;
    public string Id { get; }
    public IReadOnlyCollection<AActor> Actors => _actors;
    public AInventory Inventory { get; }

    public void DisableUserInput(bool disable)
    {
        foreach (var actor in Actors)
        {
            if (actor.ActorBody != null)
                actor.ActorBody.InputHandler.UserInputDisabled = disable;
        }
    }

    public bool ContainsActor(AActor actor) => _actors.Contains(actor);
    public bool ContainsActor(string id) => _actors.Any(x => x.ActorId == id);

    public AActor? GetActorById(string id) => _actors.Find(x => x.ActorId == id);
}

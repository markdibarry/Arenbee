using System.Collections.Generic;
using System.Linq;
using Arenbee.Items;
using GameCore.Actors;
using GameCore.Items;

namespace Arenbee.Game;

public class PlayerParty
{
    public PlayerParty()
    {
        Inventory = new Inventory();
        _actors = new();
    }

    public PlayerParty(IEnumerable<AActor> actors, AInventory inventory)
    {
        Inventory = inventory;
        _actors = actors.ToList();
    }

    private readonly List<AActor> _actors;
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

    public AActor? GetPlayerByName(string name)
    {
        return _actors.Find(x => x.Name == name);
    }
}

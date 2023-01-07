using System.Collections.Generic;
using GameCore.Actors;
using GameCore.SaveData;
using GameCore.Items;

namespace GameCore;

public class PlayerParty
{
    public PlayerParty()
    {
        _actors = new List<ActorBase>();
        Inventory = new Inventory();
    }

    public PlayerParty(IEnumerable<ActorData> actorData, ICollection<ItemStack> items)
    {
        _actors = new List<ActorBase>();
        Inventory = new Inventory(items);
        // var actor = GDEx.Instantiate<Actor>(Arenbee.Actors.Players.Twosen.GetScenePath());
        // actor.Inventory = Inventory;
        // actor.Equipment = new Equipment();
        // _actors.Add(actor);
        foreach (var data in actorData)
            _actors.Add(data.GetActor(Inventory));
    }

    private readonly List<ActorBase> _actors;
    public IReadOnlyCollection<ActorBase> Actors => _actors.AsReadOnly();
    public Inventory Inventory { get; }

    public void DisableUserInput(bool disable)
    {
        foreach (var actor in Actors)
            actor.InputHandler.UserInputDisabled = disable;
    }

    public void Free()
    {
        foreach (ActorBase actor in Actors)
            actor.QueueFree();
    }

    public ActorBase GetPlayerByName(string name)
    {
        return _actors.Find(x => x.Name == name);
    }
}

using System;
using System.Collections.Generic;

namespace GameCore.Actors;

public abstract class AActorDataDB
{
    protected AActorDataDB()
    {
        _actorData = BuildDB();
    }

    private readonly IActorData[] _actorData;
    public IReadOnlyCollection<IActorData> ActorData => _actorData;

    public IActorData? GetActorData(string id)
    {
        return Array.Find(_actorData, actorData => actorData.ActorId.Equals(id));
    }

    protected abstract IActorData[] BuildDB();
}

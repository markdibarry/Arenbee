using System;
using System.Collections.Generic;

namespace GameCore.Actors;

public abstract class AActorDataDB
{
    protected AActorDataDB()
    {
        _actorData = BuildDB();
    }

    private readonly AActorData[] _actorData;
    public IReadOnlyCollection<AActorData> ActorData => _actorData;

    public AActorData? GetActorData(string id)
    {
        return Array.Find(_actorData, actorData => actorData.ActorId.Equals(id));
    }

    protected abstract AActorData[] BuildDB();
}

using System.Collections.Generic;

namespace GameCore.Actors;

public abstract class AActorDataDB
{
    protected AActorDataDB()
    {
        _actorData = BuildDB();
    }

    private readonly Dictionary<string, AActorData> _actorData;
    public IReadOnlyDictionary<string, AActorData> ActorData => _actorData;

    public bool TryGetData<T>(string key, out T? value) where T : AActorData
    {
        if (_actorData.TryGetValue(key, out AActorData? actorData) && actorData is T t)
        {
            value = t;
            return true;
        }
        value = default;
        return false;
    }

    public T? GetData<T>(string id) where T : AActorData
    {
        if (_actorData.TryGetValue(id, out AActorData? actorData) && actorData is T t)
            return t;
        return null;
    }

    protected abstract Dictionary<string, AActorData> BuildDB();
}

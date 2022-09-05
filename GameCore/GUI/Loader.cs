using System.Collections.Generic;
using System.Linq;

namespace GameCore.GUI;

public class Loader
{
    public Loader(string path)
        : this(new string[] { path })
    { }

    public Loader(string[] paths)
    {
        _objects = new();
        foreach (string path in paths)
        {
            if (path.EndsWith("gamesave.json"))
                _objects.Add(new ObjectLoaderGameSave(path));
            else
                _objects.Add(new ObjectLoaderResource(path));
        }
    }

    private ObjectLoader _currentObject;
    private readonly List<ObjectLoader> _objects;
    public LoaderStatus Status { get; set; }

    public T GetObject<T>(string path)
    {
        var result = _objects.FirstOrDefault(x => x.Path == path)?.LoadedObject;
        if (result is not T)
            return default;
        return (T)result;
    }

    public int Load()
    {
        if (Status == LoaderStatus.Inactive)
        {
            _currentObject = GetNextObjectToLoad();
            Status = _currentObject == null ? LoaderStatus.AllLoaded : LoaderStatus.Loading;
        }

        if (Status == LoaderStatus.Loading)
        {
            _currentObject.Load();
            if (_currentObject.Status == ObjectLoadStatus.Loaded)
                Status = LoaderStatus.Inactive;
        }

        return GetProgress();
    }

    public int GetProgress()
    {
        int totalProgress = 0;
        if (_objects.Count == 0)
            return totalProgress;
        foreach (ObjectLoader obj in _objects)
        {
            totalProgress += obj.Status == ObjectLoadStatus.Loaded ? 100 : obj.Progress;
        }
        return totalProgress / _objects.Count;
    }

    public ObjectLoader GetNextObjectToLoad()
    {
        return _objects.FirstOrDefault(x => x.Status == ObjectLoadStatus.NotLoaded);
    }
}

public enum LoaderStatus
{
    Inactive,
    Loading,
    AllLoaded
}

using GameCore.Game.SaveData;
using Godot;

namespace GameCore.GUI;

public abstract class ObjectLoader
{
    protected ObjectLoader(string path)
    {
        Path = path;
    }

    public ObjectLoadStatus Status { get; set; }
    public object LoadedObject { get; set; }
    public int Progress { get; set; }
    public string Path { get; set; }

    public abstract void Load();
}

public class ObjectLoaderResource : ObjectLoader
{
    public ObjectLoaderResource(string path)
        : base(path) { }

    private readonly Godot.Collections.Array _loadProgress = new();
    private ResourceLoader.ThreadLoadStatus _loadStatus;

    public override void Load()
    {
        _loadStatus = ResourceLoader.LoadThreadedGetStatus(Path, _loadProgress);
        if (Status == ObjectLoadStatus.NotLoaded)
        {
            ResourceLoader.LoadThreadedRequest(Path);
            Status = ObjectLoadStatus.Loading;
        }
        else if (_loadStatus == ResourceLoader.ThreadLoadStatus.Loaded && Status != ObjectLoadStatus.Loaded)
        {
            LoadedObject = ResourceLoader.LoadThreadedGet(Path);
            Status = ObjectLoadStatus.Loaded;
        }
        Progress = (int)((double)_loadProgress[0] * 100);
    }
}

public class ObjectLoaderGameSave : ObjectLoader
{
    public ObjectLoaderGameSave(string path)
        : base(path) { }

    public override void Load()
    {
        if (Status == ObjectLoadStatus.NotLoaded)
        {
            LoadedObject = SaveService.LoadGame(Path);
            Status = ObjectLoadStatus.Loaded;
            Progress = 100;
        }
    }
}

public enum ObjectLoadStatus
{
    NotLoaded,
    Loading,
    Loaded
}

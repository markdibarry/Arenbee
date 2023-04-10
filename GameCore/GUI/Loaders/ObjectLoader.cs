using System;
using System.Threading.Tasks;
using Godot;

namespace GameCore.GUI;

public abstract class ObjectLoader
{
    protected ObjectLoader(string path, Action reportProgress)
    {
        Path = path;
        ReportProgress = reportProgress;
    }

    public object? LoadedObject { get; set; }
    public int Progress { get; set; }
    public string Path { get; set; }
    public Action ReportProgress { get; set; }
    public abstract Task<object?> LoadAsync();
}

public class ObjectLoaderResource : ObjectLoader
{
    public ObjectLoaderResource(string path, Action reportProgress)
        : base(path, reportProgress) { }

    public override async Task<object?> LoadAsync()
    {
        ResourceLoader.ThreadLoadStatus loadStatus;
        Godot.Collections.Array loadProgress = new();
        ResourceLoader.LoadThreadedRequest(Path);

        loadStatus = ResourceLoader.LoadThreadedGetStatus(Path, loadProgress);
        Progress = (int)((double)loadProgress[0] * 100);
        ReportProgress();

        while (loadStatus == ResourceLoader.ThreadLoadStatus.InProgress)
        {
            await Task.Delay(100);
            loadStatus = ResourceLoader.LoadThreadedGetStatus(Path, loadProgress);
            Progress = (int)((double)loadProgress[0] * 100);
            ReportProgress();
        }
        if (loadStatus == ResourceLoader.ThreadLoadStatus.Loaded)
            LoadedObject = ResourceLoader.LoadThreadedGet(Path);
        return LoadedObject;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arenbee.GUI;

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
            if (path.StartsWith(Config.SavePrefix))
                _objects.Add(new ObjectLoaderGameSave(path, OnReport));
            else
                _objects.Add(new ObjectLoaderResource(path, OnReport));
        }
    }

    private readonly List<IObjectLoader> _objects;
    public event Action<int>? ProgressUpdate;

    public T? GetObject<T>(string path)
    {
        object? result = _objects.FirstOrDefault(x => x.Path == path)?.LoadedObject;
        return result is T t ? t : default;
    }

    public async Task LoadAsync()
    {
        foreach (var obj in _objects)
            await obj.LoadAsync();
    }

    public void OnReport()
    {
        ProgressUpdate?.Invoke((int)_objects.Average(x => x.Progress));
    }
}

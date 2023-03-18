using System;
using System.Threading.Tasks;
using Arenbee.SaveData;
using GameCore.GUI;

namespace Arenbee.GUI;

public class ObjectLoaderGameSave : ObjectLoader
{
    public ObjectLoaderGameSave(string path, Action reportProgress)
        : base(path, reportProgress) { }

    public override Task<object?> LoadAsync()
    {
        LoadedObject = SaveService.GetGameSave(Path);
        Progress = 100;
        ReportProgress();
        return Task.FromResult(LoadedObject);
    }
}

using System;
using System.Threading.Tasks;
using GameCore.GUI;

namespace Arenbee.GUI;

public class GameSaveLoader : ObjectLoader
{
    public GameSaveLoader(string path, Action reportProgress)
        : base(path, reportProgress) { }

    public override Task<object?> LoadAsync()
    {
        LoadedObject = SaveService.GetGameSave(Path);
        Progress = 100;
        ReportProgress();
        return Task.FromResult(LoadedObject);
    }
}

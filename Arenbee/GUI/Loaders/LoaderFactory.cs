using System;
using GameCore;
using GameCore.GUI;

namespace Arenbee.GUI;

public class LoaderFactory : ILoaderFactory
{
    public ObjectLoader GetLoader(string path, Action reportCallback)
    {
        if (path.StartsWith(Config.SaveNamePrefix))
            return new GameSaveLoader(path, reportCallback);
        else
            return new ResourceLoader(path, reportCallback);
    }
}

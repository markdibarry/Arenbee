using System;
using GameCore;
using GameCore.GUI;

namespace Arenbee.GUI;

public class LoaderFactory : ILoaderFactory
{
    public ObjectLoader GetLoader(string path, Action reportCallback)
    {
        if (path.StartsWith(Config.SavePrefix))
            return new ObjectLoaderGameSave(path, reportCallback);
        else
            return new ObjectLoaderResource(path, reportCallback);
    }
}

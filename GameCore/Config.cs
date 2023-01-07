using System.Runtime.CompilerServices;

namespace GameCore;

public static class Config
{
    public const string ProjectDirName = "Arenbee";
    public const string ProjectPrefix = $"res://{ProjectDirName}";
    public const string PortraitsPath = $"{ProjectPrefix}/GUI/Portraits";
    public const string DialogPath = $"{ProjectPrefix}/GUI/Dialog/";
    public const string AudioPath = $"{ProjectPrefix}/Audio/";
    public const string ItemPath = $"{ProjectPrefix}/Items/";
    public const string SavePath = "user://";
    public const string SavePrefix = "gamesave";

    public static string GodotRoot => GetGodotRoot();
    private static string GetGodotRoot([CallerFilePath] string rootResourcePath = "")
    {
        int stopIndex = rootResourcePath.LastIndexOf(ProjectDirName) + ProjectDirName.Length;
        return rootResourcePath[..stopIndex];
    }
}

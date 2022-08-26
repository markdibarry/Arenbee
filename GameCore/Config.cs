using System.Runtime.CompilerServices;

namespace GameCore
{
    public static class Config
    {
        public const string ProjectDirName = "Arenbee";
        public const string AudioPath = "res://Arenbee/Audio/";
        public static string GodotRoot => GetGodotRoot();
        private static string GetGodotRoot([CallerFilePath] string rootResourcePath = "")
        {
            int stopIndex = rootResourcePath.LastIndexOf(ProjectDirName) + ProjectDirName.Length;
            return rootResourcePath[..stopIndex];
        }
    }
}

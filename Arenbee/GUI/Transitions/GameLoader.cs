using GameCore.Extensions;
using Godot;

namespace Arenbee.GUI
{
    public partial class GameLoader : Control
    {
        public static string GetScenePath() => GDEx.GetScenePath();
    }
}
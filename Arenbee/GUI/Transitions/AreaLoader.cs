using GameCore.Extensions;
using Godot;

namespace Arenbee.GUI
{
    public partial class AreaLoader : Control
    {
        public static string GetScenePath() => GDEx.GetScenePath();
    }
}
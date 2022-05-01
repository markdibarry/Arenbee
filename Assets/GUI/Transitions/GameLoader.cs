using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Assets.GUI
{
    public partial class GameLoader : Control
    {
        public static string GetScenePath() => GDEx.GetScenePath();
    }
}
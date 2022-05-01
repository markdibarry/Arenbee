using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Assets.GUI
{
    public partial class AreaLoader : Control
    {
        public static string GetScenePath() => GDEx.GetScenePath();
    }
}
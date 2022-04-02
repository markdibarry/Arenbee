using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class StateDisplay : Control
    {
        public static string GetScenePath() => GDEx.GetScenePath();
    }
}
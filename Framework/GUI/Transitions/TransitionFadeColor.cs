using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class TransitionFadeColor : ColorRect
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public AnimationPlayer AnimationPlayer { get; private set; }
        public override void _Ready()
        {
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        }
    }
}

using System.Threading.Tasks;
using Arenbee.Assets.GUI;
using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Framework.GUI
{
    public class RoomTransition : SceneTransition
    {
        public RoomTransition(TransitionData transitionData)
        {
            ScenePath = transitionData.ScenePath;
        }

        private Control _loader;

        public override void AddNewScene()
        {
            var scene = GD.Load<PackedScene>(ScenePath).Instantiate<AreaScene>();
            Locator.GetGameSession()?.AddAreaScene(scene);
        }

        public override async Task TransitionFromScene()
        {
            _loader = GD.Load<PackedScene>(AreaLoader.GetScenePath()).Instantiate<Control>();
            _loader.Modulate = Colors.Transparent;
            Locator.GetGameSession()?.Transition.AddChild(_loader);
            var tween = _loader.GetTree().CreateTween();
            tween.TweenProperty(_loader, "modulate:a", 1f, 0.2f);
            await _loader.ToSignal(tween, "finished");
        }

        public override async Task TransitionToScene()
        {
            var tween = _loader.GetTree().CreateTween();
            tween.TweenProperty(_loader, "modulate:a", 0f, 0.2f);
            await _loader.ToSignal(tween, "finished");
            Locator.GetGameSession()?.Transition.QueueFreeAllChildren();
        }
    }
}
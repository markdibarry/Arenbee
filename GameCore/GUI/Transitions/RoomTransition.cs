using System.Threading.Tasks;
using Arenbee.GUI;
using GameCore.AreaScenes;
using GameCore.Extensions;
using GameCore.Utility;
using Godot;

namespace GameCore.GUI
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
            Locator.Session?.AddAreaScene(scene);
        }

        public override async Task TransitionFromScene()
        {
            _loader = GD.Load<PackedScene>(AreaLoader.GetScenePath()).Instantiate<Control>();
            _loader.Modulate = Colors.Transparent;
            Locator.Session?.Transition.AddChild(_loader);
            var tween = _loader.GetTree().CreateTween();
            var prop = tween.TweenProperty(_loader, "modulate:a", 1f, 0.2f);
            await _loader.ToSignal(tween, "finished");
            tween.Dispose();
            prop.Dispose();
        }

        public override async Task TransitionToScene()
        {
            var tween = _loader.GetTree().CreateTween();
            var prop = tween.TweenProperty(_loader, "modulate:a", 0f, 0.2f);
            await _loader.ToSignal(tween, "finished");
            tween.Dispose();
            prop.Dispose();
            Locator.Session?.Transition.QueueFreeAllChildren();
        }
    }
}
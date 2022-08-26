using System.Threading.Tasks;
using Arenbee.GUI;
using GameCore.AreaScenes;
using GameCore.Extensions;
using GameCore.Utility;
using Godot;

namespace GameCore.GUI
{
    public class AreaTransition : SceneTransition
    {
        public AreaTransition(TransitionData transitionData)
        {
            ScenePath = transitionData.ScenePath;
        }

        public override void AddNewScene()
        {
            var scene = GD.Load<PackedScene>(ScenePath).Instantiate<AreaScene>();
            Locator.Session?.AddAreaScene(scene);
        }

        public override Task TransitionFromScene()
        {
            var loader = GD.Load<PackedScene>(AreaLoader.GetScenePath()).Instantiate<Control>();
            Locator.Root?.Transition.AddChild(loader);
            return Task.CompletedTask;
        }

        public override Task TransitionToScene()
        {
            Locator.Root?.Transition.QueueFreeAllChildren();
            return Task.CompletedTask;
        }
    }
}

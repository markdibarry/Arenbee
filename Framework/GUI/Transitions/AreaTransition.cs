using System.Threading.Tasks;
using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Framework.GUI
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
            Locator.GetGameSession()?.AddAreaScene(scene);
        }

        public override Task TransitionFromScene()
        {
            var loader = GD.Load<PackedScene>("res://Assets/GUI/Transitions/AreaLoader.tscn").Instantiate<Control>();
            GameRoot.Instance.Transition.AddChild(loader);
            return Task.CompletedTask;
        }

        public override Task TransitionToScene()
        {
            GameRoot.Instance.Transition.QueueFreeAllChildren();
            return Task.CompletedTask;
        }
    }
}
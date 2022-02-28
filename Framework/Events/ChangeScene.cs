using Arenbee.Framework.Actors;
using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Utility;
using Godot;

namespace Arenbee.Framework.Events
{
    public partial class ChangeScene : Area2D
    {
        [Export(PropertyHint.File)]
        public string PackedScenePath { get; set; }
        public override void _Ready()
        {
            BodyEntered += OnBodyEntered;
        }
        public void OnBodyEntered(Node body)
        {
            if (PackedScenePath == null) return;
            if (body is Actor actor && actor.ActorType == ActorType.Player)
            {
                AreaScene newScene = GD.Load<PackedScene>(PackedScenePath).Instantiate<AreaScene>();
                Locator.GetCurrentGame().ReplaceScene(newScene);
            }
        }
    }
}

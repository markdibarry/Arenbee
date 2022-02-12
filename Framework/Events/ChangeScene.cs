using Arenbee.Framework.Actors;
using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Game;
using Godot;

namespace Arenbee.Framework.Events
{
    public partial class ChangeScene : Area2D
    {
        [Export(PropertyHint.File)]
        private string _packedScenePath;
        public override void _Ready()
        {
            BodyEntered += OnBodyEntered;
        }
        public void OnBodyEntered(Node body)
        {
            if (_packedScenePath == null) return;
            if (body is Actor actor && actor.ActorType == ActorType.Player)
            {
                AreaScene newScene = GD.Load<PackedScene>(_packedScenePath).Instantiate<AreaScene>();
                GameRoot.Instance.CurrentGame.ReplaceScene(newScene);
            }
        }
    }
}

using GameCore.Actors;
using GameCore.Game;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace GameCore.Events
{
    public partial class ChangeScene : Area2D
    {
        [Export(PropertyHint.File)]
        public string PackedScenePath { get; set; }
        [Export(PropertyHint.Enum)]
        public TransitionType TransitionType { get; set; }
        public bool IsActive { get; set; }

        public override void _Ready()
        {
            BodyEntered += OnBodyEntered;
        }

        public void OnBodyEntered(Node body)
        {
            if (IsActive || PackedScenePath == null) return;
            if (body is Actor actor && actor.ActorType == ActorType.Player)
            {
                IsActive = true;
                if (!File.FileExists(PackedScenePath))
                    return;
                CallDeferred(nameof(LoadScene));
            }
        }

        private void LoadScene()
        {
            var tData = new TransitionData()
            {
                ScenePath = PackedScenePath,
                TransitionType = TransitionType
            };
            var tController = Locator.TransitionController;
            tController.LoadScene(tData);
        }
    }
}

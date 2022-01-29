using Arenbee.Framework.GUI;
using Godot;

namespace Arenbee.Framework.AreaScenes
{
    public partial class AreaScene : Node
    {
        public HUD HUD { get; set; }
        public Position2D PlayerSpawnPoint { get; set; }
        public Node2D PlayersContainer { get; set; }

        public override void _Ready()
        {
            SetNodeReferences();
            Init();
        }

        private void SetNodeReferences()
        {
            HUD = GetNodeOrNull<HUD>("HUD");
            PlayersContainer = GetNodeOrNull<Node2D>("Players");
            PlayerSpawnPoint = GetNodeOrNull<Position2D>("PlayerSpawnPoint");
        }

        private void Init()
        {
        }
    }
}

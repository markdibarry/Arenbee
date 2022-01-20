using System.Linq;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Menus.HUD;
using Godot;

namespace Arenbee.Assets
{
    public partial class DemoLevel : Node
    {
        public HUD HUD { get; set; }
        public override void _Ready()
        {
            SetNodeReferences();
            Init();
        }

        private void SetNodeReferences()
        {
            HUD = GetNodeOrNull<HUD>("HUD");
        }

        private void Init()
        {
            if (HUD != null)
            {
                var players = GetNodeOrNull<Node>("Players")
                    .GetChildren().OfType<Actor>();
                var enemies = GetNodeOrNull<Node>("Enemies")
                    .GetChildren().OfType<Actor>();
                HUD.SubscribeToEvents(players);
                HUD.SubscribeToEvents(enemies);
            }
        }
    }
}

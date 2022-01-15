using System.Collections.Generic;
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
                var actors = new List<Actor>();
                var players = GetNodeOrNull<Node>("Players")?
                    .GetChildren()
                    .OfType<Player>()
                    .ToList();
                var enemies = GetNodeOrNull<Node>("Enemies")?
                    .GetChildren()
                    .OfType<Enemy>()
                    .ToList();
                if (players != null) actors.AddRange(players);
                if (enemies != null) actors.AddRange(enemies);
                HUD.SubscribeToEvents(actors);
            }
        }
    }
}

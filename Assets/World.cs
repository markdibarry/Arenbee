using System;
using Arenbee.Framework.Constants;
using Godot;

namespace Arenbee.Assets
{
    public partial class World : Node
    {
        public bool LevelLoaded { get; set; }
        public DemoLevel CurrentLevel { get; set; }

        public override void _Ready()
        {
            CurrentLevel = GetNodeOrNull<DemoLevel>("DemoLevel");
            if (CurrentLevel != null) LevelLoaded = true;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            if (Godot.Input.IsActionJustPressed("Load"))
            {
                if (LevelLoaded)
                {
                    CurrentLevel.QueueFree();
                    LevelLoaded = false;
                }
                else
                {
                    LevelLoaded = true;
                    PackedScene demoLevelScene = ResourceLoader.Load<PackedScene>(PathConstants.DemoLevel);
                    CurrentLevel = demoLevelScene.Instantiate<DemoLevel>();
                    AddChild(CurrentLevel);
                }
            }

            if (Godot.Input.IsActionJustPressed("pause"))
            {
                GetTree().Paused = !GetTree().Paused;
            }

            if (Godot.Input.IsActionJustPressed("collect"))
            {
                GC.Collect();
            }

            if (Godot.Input.IsActionJustPressed("print"))
            {
                PrintStrayNodes();
            }
        }
    }
}

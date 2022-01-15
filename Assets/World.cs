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
            if (Input.IsActionJustPressed("Load"))
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

            if (Input.IsActionJustPressed("pause"))
            {
                GetTree().Paused = !GetTree().Paused;
            }

            if (Input.IsActionJustPressed("collect"))
            {
                GC.Collect();
            }

        }
    }
}

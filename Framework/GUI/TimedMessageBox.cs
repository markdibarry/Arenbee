using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class TimedMessageBox : MessageBox
    {
        public static new readonly string ScenePath = $"res://Framework/GUI/{nameof(TimedMessageBox)}.tscn";
        [Export]
        public float TimeOut { get; set; } = 2.0f;
        public bool _timerFinished;

        public override void _Ready()
        {
            base._Ready();
            if (TimeOut <= 0)
                _timerFinished = true;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Engine.IsEditorHint()) return;
            base._PhysicsProcess(delta);
            if (!_timerFinished)
            {
                if (TimeOut > 0)
                {
                    TimeOut -= delta;
                }
                else
                {
                    _timerFinished = true;
                    QueueFree();
                }
            }

        }
    }
}

using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class TimedMessageBox : MessageBox
    {
        public static new readonly string ScenePath = $"res://Framework/GUI/{nameof(TimedMessageBox)}.tscn";
        [Export]
        private float _timeOut = 2.0f;
        private bool _timerFinished;

        public override void _Ready()
        {
            base._Ready();
            if (_timeOut <= 0)
                _timerFinished = true;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Engine.IsEditorHint()) return;
            base._PhysicsProcess(delta);
            if (!_timerFinished)
            {
                if (_timeOut > 0)
                {
                    _timeOut -= delta;
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

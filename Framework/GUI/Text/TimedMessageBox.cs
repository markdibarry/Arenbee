using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Framework.GUI.Text
{
    [Tool]
    public partial class TimedMessageBox : MessageBox
    {
        public static new string GetScenePath() => GDEx.GetScenePath();
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

using System;
using GameCore.Extensions;
using Godot;

namespace GameCore.GUI.Text
{
    [Tool]
    public partial class TimedMessageBox : MessageBox
    {
        public static new string GetScenePath() => GDEx.GetScenePath();
        [Export]
        private float _timeOut = 2.0f;
        private bool _timerFinished;

        public override void _Process(float delta)
        {
            if (Engine.IsEditorHint())
                return;
            base._PhysicsProcess(delta);
            if (_timerFinished)
                return;
            if (_timeOut > 0)
            {
                _timeOut -= delta;
            }
            else
            {
                _timerFinished = true;
                TransitionOut();
            }
        }

        public async void TransitionOut()
        {
            using Tween fadeTween = GetTree().CreateTween();
            using PropertyTweener fade = fadeTween.TweenProperty(this, "modulate:a", 0f, 0.1f);
            await ToSignal(fadeTween, "finished");
            QueueFree();
        }
    }
}

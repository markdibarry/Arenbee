using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class PromptSubMenu : SubMenu
    {
        [Export]
        private float _timeDuration;
        private bool _timerEnabled;

        public override void _Process(float delta)
        {
            if (Engine.IsEditorHint()) return;
            base._Process(delta);

            if (MenuInput.Enter.IsActionJustPressed)
                Confirm();

            if (_timerEnabled)
            {
                if (_timeDuration < 0)
                    OnTimeOut();
                else
                    _timeDuration -= delta;
            }
        }

        protected override void PreLoadSetup()
        {
            if (_timeDuration > 0) _timerEnabled = true;
            base.PreLoadSetup();
        }

        protected virtual void Confirm() { }

        protected virtual void OnTimeOut()
        {
            _timerEnabled = false;
        }
    }
}

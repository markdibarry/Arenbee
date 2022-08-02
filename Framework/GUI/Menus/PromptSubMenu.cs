using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class PromptSubMenu : SubMenu
    {
        [Export]
        private float _timeDuration;
        private bool _timerEnabled;

        public override void HandleInput(GUIInputHandler menuInput, float delta)
        {
            base.HandleInput(menuInput, delta);

            if (menuInput.Enter.IsActionJustPressed)
                Confirm();

            if (_timerEnabled)
            {
                if (_timeDuration < 0)
                    OnTimeOut();
                else
                    _timeDuration -= delta;
            }
        }

        protected virtual void Confirm() { }

        protected override void PreWaitFrameSetup()
        {
            if (_timeDuration > 0) _timerEnabled = true;
            base.PreWaitFrameSetup();
        }

        protected virtual void OnTimeOut()
        {
            _timerEnabled = false;
        }
    }
}

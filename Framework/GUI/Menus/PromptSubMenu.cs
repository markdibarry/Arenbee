using System.Threading.Tasks;
using Arenbee.Framework.Game;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class PromptSubMenu : SubMenu
    {
        [Export]
        private float _timeDuration;
        private bool _timerEnabled;

        public override async Task CustomSubMenuSetup()
        {
            if (_timeDuration > 0) _timerEnabled = true;
            await base.CustomSubMenuSetup();
        }

        public override void _Process(float delta)
        {
            if (Engine.IsEditorHint()) return;
            base._Process(delta);

            if (GameRoot.MenuInput.Enter.IsActionJustPressed)
                Confirm();

            if (_timerEnabled)
            {
                if (_timeDuration < 0)
                {
                    OnTimeOut();
                    _timerEnabled = false;
                }
                else
                {
                    _timeDuration -= delta;
                }
            }
        }

        protected virtual void Confirm() { }

        protected virtual void OnTimeOut() { }
    }
}

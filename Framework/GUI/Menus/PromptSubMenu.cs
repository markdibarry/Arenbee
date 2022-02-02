using System.Threading.Tasks;
using Arenbee.Framework.Game;
using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class PromptSubMenu : SubMenu
    {
        [Export]
        protected float Timer { get; set; }
        private bool _timerEnabled;

        protected override Task Init()
        {
            if (Timer > 0) _timerEnabled = true;
            return Task.CompletedTask;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Engine.IsEditorHint()) return;
            base._PhysicsProcess(delta);

            var menuInput = GameRoot.MenuInput;

            if (menuInput.Enter.IsActionJustPressed)
            {
                Confirm();
            }

            if (_timerEnabled)
            {
                if (Timer < 0)
                {
                    OnTimeOut();
                    _timerEnabled = false;
                }
                else
                {
                    Timer -= delta;
                }
            }
        }

        public virtual void Confirm() { }

        public virtual void OnTimeOut() { }
    }
}

using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.SaveData;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class SaveSuccessSubMenu : SubMenu
    {
        private float _timer = 2.0f;
        private bool _timerFinished;

        public override void _Ready()
        {
            base._Ready();
            PreventCloseAll = true;
            PreventCancel = true;
        }

        public override void OnItemSelected(OptionItem optionItem)
        {
            base.OnItemSelected(optionItem);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            if (!_timerFinished && _timer < 0)
            {
                RaiseRequestedRemoveSubMenu();
                _timerFinished = true;
            }
            _timer -= delta;
        }

    }
}

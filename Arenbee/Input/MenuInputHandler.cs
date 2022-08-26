using GameCore.Input;

namespace Arenbee.Input
{
    public partial class MenuInputHandler : GUIInputHandler
    {
        protected override void SetInputActions()
        {
            Start = new InputAction(this, "menu_start");
            Enter = new InputAction(this, "menu_enter");
            Cancel = new InputAction(this, "menu_cancel");
            Up = new InputAction(this, "menu_up");
            Down = new InputAction(this, "menu_down");
            Left = new InputAction(this, "menu_left");
            Right = new InputAction(this, "menu_right");
        }
    }
}

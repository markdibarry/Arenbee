using GameCore.Input;

namespace Arenbee.Input;

public class MenuInputHandler : GUIInputHandler
{
    public MenuInputHandler()
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

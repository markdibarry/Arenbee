using GameCore.Input;

namespace Arenbee.Input;

public class MenuInputHandler : GUIInputHandler
{
    public MenuInputHandler()
    {
        Accept = new UserInputAction(this, "menu_accept");
        Cancel = new UserInputAction(this, "menu_cancel");
        Start = new UserInputAction(this, "menu_start");
        Up = new UserInputAction(this, "menu_up");
        Down = new UserInputAction(this, "menu_down");
        Left = new UserInputAction(this, "menu_left");
        Right = new UserInputAction(this, "menu_right");
    }

    public override InputAction Accept { get; }
    public override InputAction Cancel { get; }
    public override InputAction Start { get; }
    public override InputAction Up { get; }
    public override InputAction Down { get; }
    public override InputAction Left { get; }
    public override InputAction Right { get; }
}

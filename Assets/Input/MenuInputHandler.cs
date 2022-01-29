namespace Arenbee.Framework.Input
{
    public partial class MenuInputHandler : GUIInputHandler
    {
        public MenuInputHandler()
        {
            Start = new InputAction("menu_start");
            Enter = new InputAction("menu_enter");
            Cancel = new InputAction("menu_cancel");
            Up = new InputAction("menu_up");
            Down = new InputAction("menu_down");
            Left = new InputAction("menu_left");
            Right = new InputAction("menu_right");
        }
    }
}

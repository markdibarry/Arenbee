using GameCore.Input;

namespace Arenbee.Input;
public static class InputFactory
{
    public static IActorInputHandler CreateAIInput()
    {
        return new ActorInputHandler(
            jump: "",
            attack: "",
            subAction: "",
            run: "",
            up: "",
            down: "",
            left: "",
            right: "");
    }

    public static IActorInputHandler CreatePlayer1Input()
    {
        return new ActorInputHandler(
            jump: "p1_jump",
            attack: "p1_attack",
            subAction: "p1_subAction",
            run: "p1_run",
            up: "p1_up",
            down: "p1_down",
            left: "p1_left",
            right: "p1_right");
    }

    public static IGUIInputHandler CreateMenuInput()
    {
        return new GUIInputHandler(
            accept: "menu_accept",
            cancel: "menu_cancel",
            start: "menu_start",
            up: "menu_up",
            down: "menu_down",
            left: "menu_left",
            right: "menu_right");
    }
}

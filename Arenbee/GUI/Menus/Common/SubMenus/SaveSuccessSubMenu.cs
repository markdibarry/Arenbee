using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class SaveSuccessSubMenu : PromptSubMenu
{
    public SaveSuccessSubMenu()
    {
        PreventCloseAll = true;
        PreventCancel = true;
    }

    public static string GetScenePath() => GDEx.GetScenePath();

    protected override void OnTimeOut()
    {
        base.OnTimeOut();
        RequestCloseSubMenu(new GUICloseRequest() { CascadeTo = typeof(SaveGameSubMenu) });
    }
}

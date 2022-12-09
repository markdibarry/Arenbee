using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class SaveSuccessSubMenu : PromptSubMenu
{
    public SaveSuccessSubMenu()
    {

    }

    public static string GetScenePath() => GDEx.GetScenePath();

    protected override void OnTimeOut()
    {
        base.OnTimeOut();
        CloseSubMenuAsync(cascadeTo: typeof(SaveGameSubMenu));
    }

    protected override void CustomSetup()
    {
        PreventCloseAll = true;
        PreventCancel = true;
    }
}

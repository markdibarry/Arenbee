using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class SaveSuccessSubMenu : PromptSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();

    protected override void OnTimeOut()
    {
        base.OnTimeOut();
        _ = CloseSubMenuAsync(cascadeTo: typeof(SaveGameSubMenu));
    }

    protected override void OnSetup()
    {
        PreventCloseAll = true;
        PreventCancel = true;
    }
}

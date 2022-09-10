using GameCore.Extensions;
using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameCore.GUI.Dialogs;

[Tool]
public partial class DialogOptionMenu : Menu
{
    public static string GetScenePath() => GDEx.GetScenePath();
    private readonly PackedScene _dialogOptionSubMenuScene = GD.Load<PackedScene>(DialogOptionSubMenu.GetScenePath());
    public DialogChoice[] DialogChoices { get; set; }
    public Dialog Dialog { get; set; }

    public override void DataTransfer(Dictionary<string, object> grabBag)
    {
        DialogChoices = (DialogChoice[])grabBag["DialogChoices"];
        Dialog = (Dialog)grabBag["Dialog"];
    }

    public override async Task TransitionOpenAsync()
    {
        var dialogOptionSubMenu = _dialogOptionSubMenuScene.Instantiate<DialogOptionSubMenu>();
        dialogOptionSubMenu.DialogChoices = DialogChoices;
        dialogOptionSubMenu.Dialog = Dialog;
        await AddSubMenuAsync(dialogOptionSubMenu);
    }
}

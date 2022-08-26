using GameCore.Extensions;
using Godot;
using System.Threading.Tasks;

namespace GameCore.GUI.Dialogs
{
    [Tool]
    public partial class DialogOptionMenu : Menu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        private readonly PackedScene _dialogOptionSubMenuScene = GD.Load<PackedScene>(DialogOptionSubMenu.GetScenePath());
        public DialogChoice[] DialogChoices { get; set; }
        public Dialog Dialog { get; set; }

        public override async Task TransitionOpenAsync()
        {
            var dialogOptionSubMenu = _dialogOptionSubMenuScene.Instantiate<DialogOptionSubMenu>();
            dialogOptionSubMenu.DialogChoices = DialogChoices;
            dialogOptionSubMenu.Dialog = Dialog;
            await AddSubMenuAsync(dialogOptionSubMenu);
        }
    }
}

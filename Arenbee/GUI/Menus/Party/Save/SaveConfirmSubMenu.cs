using GameCore.Extensions;
using GameCore.GUI;
using GameCore.SaveData;
using Godot;
using GameCore.Utility;

namespace Arenbee.GUI.Menus.Party;

[Tool]
public partial class SaveConfirmSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();

    protected override void OnItemSelected()
    {
        var saveChoice = CurrentContainer.CurrentItem.GetData<string>("saveChoice");
        if (saveChoice == null)
            return;
        switch (saveChoice)
        {
            case "Yes":
                SaveGame();
                break;
            case "No":
                RequestCloseSubMenu(new());
                break;
        }
    }

    private void SaveGame()
    {
        SaveService.SaveGame(Locator.Session);
        RequestOpenSubMenu(new GUIOpenRequest(SaveSuccessSubMenu.GetScenePath()));
    }
}

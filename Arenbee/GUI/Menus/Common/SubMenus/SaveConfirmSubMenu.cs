using GameCore.Extensions;
using GameCore.GUI;
using GameCore.SaveData;
using Godot;
using GameCore.Utility;
using System.Collections.Generic;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class SaveConfirmSubMenu : OptionSubMenu
{
    private int _gameSaveId;
    private OptionContainer _saveOptions;
    public static string GetScenePath() => GDEx.GetScenePath();

    public override void ReceiveData(object data)
    {
        if (data is not int gameSaveId)
            return;
        _gameSaveId = gameSaveId;
    }

    protected override void SetupOptions()
    {
        _saveOptions = OptionContainers.Find(x => x.Name == "SaveOptions");
        var options = GetMenuOptions();
        _saveOptions.ReplaceChildren(options);
    }

    protected override void OnItemSelected()
    {
        var saveChoice = CurrentContainer.CurrentItem.GetData<string>("value");
        if (saveChoice == null)
            return;
        switch (saveChoice)
        {
            case SaveConfirmOptions.Yes:
                SaveGame();
                break;
            case SaveConfirmOptions.No:
                RequestCloseSubMenu(new());
                break;
        }
    }

    private void SaveGame()
    {
        SaveService.SaveGame(_gameSaveId, Locator.Session);
        RequestOpenSubMenu(new GUIOpenRequest(SaveSuccessSubMenu.GetScenePath()));
    }

    private static List<TextOption> GetMenuOptions()
    {
        var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        var options = new List<TextOption>();
        foreach (var optionString in SaveConfirmOptions.GetAll())
        {
            var option = textOptionScene.Instantiate<TextOption>();
            option.LabelText = optionString;
            option.OptionData["value"] = optionString;
            options.Add(option);
        }
        return options;
    }

    private static class SaveConfirmOptions
    {
        public static List<string> GetAll()
        {
            return new List<string>()
            {
                Yes,
                No
            };
        }
        public const string Yes = "Yes";
        public const string No = "No";
    }
}

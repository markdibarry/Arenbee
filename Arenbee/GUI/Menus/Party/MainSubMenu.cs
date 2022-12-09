using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using Arenbee.GUI.Menus.Common;
using Arenbee.GUI.Menus.Party.Equipment;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Utility;
using Godot;
using Arenbee.Localization;
using System.Collections;
using System.Linq;

namespace Arenbee.GUI.Menus.Party;

[Tool]
public partial class MainSubMenu : OptionSubMenu
{
    public static string GetScenePath() => GDEx.GetScenePath();

    private OptionContainer _optionList;

    protected override void OnItemSelected()
    {
        var subMenuName = CurrentContainer.CurrentItem.GetData<string>("value");
        if (subMenuName == null)
            return;

        switch (subMenuName)
        {
            case nameof(PartyMenuOptions.KEY01Stats):
                OpenStatsSubMenu();
                break;
            case nameof(PartyMenuOptions.KEY02Inventory):
                OpenInventorySubMenu();
                break;
            case nameof(PartyMenuOptions.KEY03Equipment):
                OpenEquipmentSubMenu();
                break;
            case nameof(PartyMenuOptions.KEY05Save):
                OpenSaveGameConfirm();
                break;
            case nameof(PartyMenuOptions.KEY06Quit):
                QuitToTitle();
                break;
        }
    }

    protected override void SetupOptions()
    {
        var options = GetMenuOptions();
        _optionList.ReplaceChildren(options);
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _optionList = OptionContainers.Find(x => x.Name == "OptionContainer");
    }

    private static List<TextOption> GetMenuOptions()
    {
        var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        var options = new List<TextOption>();
        var resourceSet = PartyMenuOptions.ResourceManager.GetEntries()
            .OrderBy(i => i.Key);
        foreach (DictionaryEntry entry in resourceSet)
        {
            var option = textOptionScene.Instantiate<TextOption>();
            option.LabelText = entry.Value.ToString();
            var optionValue = entry.Key.ToString();
            option.OptionData["value"] = optionValue;
            if (optionValue == PartyMenuOptions.KEY04Options)
                option.Disabled = true;
            options.Add(option);
        }
        return options;
    }

    private void OpenStatsSubMenu()
    {
        _ = OpenSubMenuAsync(StatsSubMenu.GetScenePath());
    }

    private void OpenInventorySubMenu()
    {
        _ = OpenSubMenuAsync(InventorySubMenu.GetScenePath());
    }

    private void OpenEquipmentSubMenu()
    {
        _ = OpenSubMenuAsync(EquipmentSubMenu.GetScenePath());
    }

    private void OpenSaveGameConfirm()
    {
        _ = OpenSubMenuAsync(SaveGameSubMenu.GetScenePath());
    }

    private void QuitToTitle()
    {
        Loading = true;
        var tController = Locator.TransitionController;
        var request = new TransitionRequest(
            BasicLoadingScreen.GetScenePath(),
            TransitionType.Game,
            WipeTransition.GetScenePath(),
            FadeTransition.GetScenePath(),
            new string[] { Locator.Root?.TitleMenuScenePath },
            (loader) =>
            {
                var titleMenuScene = loader.GetObject<PackedScene>(Locator.Root?.TitleMenuScenePath);
                Locator.Root?.ResetToTitleScreenAsync(titleMenuScene);
                return Task.CompletedTask;
            });
        tController.RequestTransition(request);
    }
}

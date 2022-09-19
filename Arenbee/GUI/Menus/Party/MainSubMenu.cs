using System.Collections.Generic;
using System.Threading.Tasks;
using Arenbee.GUI.Menus.Party.Equipment;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

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
            case PartyMenuOptions.Stats:
                OpenStatsSubMenu();
                break;
            case PartyMenuOptions.Inventory:
                OpenInventorySubMenu();
                break;
            case PartyMenuOptions.Equipment:
                OpenEquipmentSubMenu();
                break;
            case PartyMenuOptions.Save:
                OpenSaveGameConfirm();
                break;
            case PartyMenuOptions.Quit:
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
        foreach (var optionString in PartyMenuOptions.GetAll())
        {
            var option = textOptionScene.Instantiate<TextOption>();
            option.LabelText = optionString;
            option.OptionData["value"] = optionString;
            if (optionString == PartyMenuOptions.Options)
                option.Disabled = true;
            options.Add(option);
        }
        return options;
    }

    private void OpenStatsSubMenu()
    {
        RequestOpenSubMenu(new GUIOpenRequest(StatsSubMenu.GetScenePath()));
    }

    private void OpenInventorySubMenu()
    {
        RequestOpenSubMenu(new GUIOpenRequest(InventorySubMenu.GetScenePath()));
    }

    private void OpenEquipmentSubMenu()
    {
        RequestOpenSubMenu(new GUIOpenRequest(EquipmentSubMenu.GetScenePath()));
    }

    private void OpenSaveGameConfirm()
    {
        RequestOpenSubMenu(new GUIOpenRequest(SaveConfirmSubMenu.GetScenePath()));
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

    private static class PartyMenuOptions
    {
        public static List<string> GetAll()
        {
            return new List<string>()
            {
                Stats,
                Inventory,
                Equipment,
                Options,
                Save,
                Quit
            };
        }
        public const string Stats = "Stats";
        public const string Inventory = "Inventory";
        public const string Equipment = "Equipment";
        public const string Options = "Options";
        public const string Save = "Save";
        public const string Quit = "Quit";
    }
}

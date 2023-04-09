using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arenbee.Actors;
using Arenbee.Game;
using Arenbee.GUI.Menus.Common;
using Arenbee.Items;
using Arenbee.SaveData;
using GameCore;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Title;

[Tool]
public partial class MainSubMenu : OptionSubMenu
{
    private OptionContainer _startOptions = null!;
    private AGameRoot _gameRoot = Locator.Root;
    private readonly List<string> _menuKeys = new()
    {
        Localization.Menus.Menus_Title_Continue,
        Localization.Menus.Menus_Title_NewGame
    };
    public static string GetScenePath() => GDEx.GetScenePath();

    protected override async Task AnimateOpenAsync()
    {
        Vector2 pos = (Size - _startOptions.Size) * 0.5f;
        _startOptions.Position = new Vector2(pos.X, -_startOptions.Size.Y);
        var tween = _startOptions.CreateTween();
        tween.TweenProperty(_startOptions, "position:y", pos.Y, 0.4f);
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    protected override void CustomSetup()
    {
        PreventCancel = true;
        PreventCloseAll = true;
    }

    protected override void OnItemSelected()
    {
        if (!CurrentContainer.FocusedItem.TryGetData("value", out string? titleChoice))
            return;
        switch (titleChoice)
        {
            case nameof(Localization.Menus.Menus_Title_Continue):
                OpenContiueSaveSubMenu();
                break;
            case nameof(Localization.Menus.Menus_Title_NewGame):
                StartNewGame();
                break;
        }
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _startOptions = OptionContainers.Find(x => x.Name == "MainOptions");
    }

    protected override void SetupOptions()
    {
        var options = GetMenuOptions();
        _startOptions.ReplaceChildren(options);
    }

    private List<TextOption> GetMenuOptions()
    {
        PackedScene textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        List<TextOption> options = new();
        List<(string, GameSave)> gameSaves = SaveService.GetAllSaves();
        foreach (string menuKey in _menuKeys)
        {
            var option = textOptionScene.Instantiate<TextOption>();
            option.LabelText = Tr(menuKey);
            option.OptionData["value"] = menuKey;
            if (menuKey == Localization.Menus.Menus_Title_Continue && gameSaves.Count == 0)
                option.Disabled = true;
            options.Add(option);
        }
        return options;
    }

    private void StartNewGame()
    {
        CurrentState = State.Busy;
        TransitionControllerBase tController = Locator.TransitionController;
        TransitionRequest request = new(
            loadingScreenPath: BasicLoadingScreen.GetScenePath(),
            transitionType: TransitionType.Game,
            transitionA: FadeTransition.GetScenePath(),
            transitionB: FadeTransition.GetScenePath(),
            paths: Array.Empty<string>(),
            callback: async (loader) =>
            {
                GameSave gameSave = GetNewGame();
                await _gameRoot.RemoveSession();
                await _gameRoot.StartNewSession(gameSave);
            });
        tController.RequestTransition(request);
    }

    private void OpenContiueSaveSubMenu()
    {
        _ = OpenSubMenuAsync(LoadGameSubMenu.GetScenePath());
    }

    private GameSave GetNewGame()
    {
        ActorData actorData = Locator.ActorDataDB.GetData<ActorData>(ActorDataIds.Twosen)!;
        return new GameSave(
            id: 0,
            lastModifiedUtc: DateTime.UtcNow,
            sessionState: new SessionState(),
            mainPartyId: "default",
            parties: new PartyData[]
            {
                new PartyData(
                    partyId: "default",
                    actorData: new ActorData[] { actorData },
                    inventoryIndex: 0,
                    items: Array.Empty<ItemStackData>()
                )
            },
            inventories: new InventoryData[]
            {
                new InventoryData(
                    new ItemStackData[]
                    {
                        new ItemStackData(ItemIds.HockeyStick, 1)
                    })
            });
    }
}

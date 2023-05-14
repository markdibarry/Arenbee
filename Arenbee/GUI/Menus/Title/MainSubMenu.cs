using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arenbee.Actors;
using Arenbee.GUI.Menus.Common;
using Arenbee.Items;
using GameCore;
using GameCore.Actors;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Title;

[Tool]
public partial class MainSubMenu : OptionSubMenu
{
    private OptionContainer _mainOptions = null!;
    private BaseGameRoot _gameRoot = Locator.Root;
    private Control _mainContainer = null!;
    private readonly List<string> _menuKeys = new()
    {
        Localization.Menus.Menus_Title_Continue,
        Localization.Menus.Menus_Title_NewGame
    };
    public static string GetScenePath() => GDEx.GetScenePath();

    protected override async Task AnimateOpenAsync()
    {
        Vector2 pos = (Size - _mainContainer.Size) * 0.5f;
        _mainContainer.Position = new Vector2(pos.X, -_mainContainer.Size.Y);
        Tween tween = CreateTween();
        tween.TweenProperty(_mainContainer, "position:y", pos.Y, 0.4f);
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    protected override void OnSetup()
    {
        SetNodeReferences();
        PreventCancel = true;
        PreventCloseAll = true;
        List<TextOption> options = GetMenuOptions();
        _mainOptions.ReplaceChildren(options);
    }

    protected override void OnSelectPressed()
    {
        if (CurrentContainer?.FocusedItem?.OptionData is not string titleChoice)
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

    private void SetNodeReferences()
    {
        _mainContainer = GetNode<Control>("%MainContainer");
        _mainOptions = GetNode<OptionContainer>("%MainOptions");
        AddContainer(_mainOptions);
    }

    private List<TextOption> GetMenuOptions()
    {
        PackedScene textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        List<TextOption> options = new();
        List<(string, GameSave)> gameSaves = SaveService.GetAllSaves();
        foreach (string menuKey in _menuKeys)
        {
            var option = textOptionScene.Instantiate<TextOption>();
            option.LabelText = this.TrS(menuKey);
            option.OptionData = menuKey;
            if (menuKey == Localization.Menus.Menus_Title_Continue && gameSaves.Count == 0)
                option.Disabled = true;
            options.Add(option);
        }
        return options;
    }

    private void StartNewGame()
    {
        CurrentState = State.Busy;
        BaseTransitionController tController = Locator.TransitionController;
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

    private static GameSave GetNewGame()
    {
        ActorData actorData = ActorsLocator.ActorDataDB.GetData<ActorData>(ActorDataIds.Twosen)!;
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

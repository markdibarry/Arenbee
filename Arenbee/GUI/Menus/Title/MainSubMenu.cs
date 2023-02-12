using System.Collections.Generic;
using System.Threading.Tasks;
using Arenbee.Actors.Players;
using Arenbee.GUI.Menus.Common;
using Arenbee.SaveData;
using GameCore;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Items;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Title;

[Tool]
public partial class MainSubMenu : OptionSubMenu
{
    private OptionContainer _startOptions;
    private GameRoot _gameRoot = (GameRoot)Locator.Root;
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
        var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        var options = new List<TextOption>();
        var gameSaves = SaveService.GetGameSaves();
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
        async Task Callback(Loader loader)
        {
            var sessionScene = loader.GetObject<PackedScene>(_gameRoot.GameSessionScenePath);
            var twosenScene = loader.GetObject<PackedScene>(Twosen.GetScenePath());
            List<AActorBody> actors = new() { twosenScene.Instantiate<Twosen>() };
            List<AItemStack> items = new() { new ItemStack("HockeyStick", 1) };
            GameSave gameSave = new(actors, items);
            var session = sessionScene.Instantiate<AGameSession>();
            await _gameRoot.RemoveSession();
            await _gameRoot.StartNewSession(session, gameSave);
        }

        var tController = Locator.TransitionController;
        var request = new TransitionRequest(
            BasicLoadingScreen.GetScenePath(),
            TransitionType.Game,
            FadeTransition.GetScenePath(),
            FadeTransition.GetScenePath(),
            new string[] { _gameRoot.GameSessionScenePath, Twosen.GetScenePath() },
            Callback);
        tController.RequestTransition(request);
    }

    private void OpenContiueSaveSubMenu()
    {
        _ = OpenSubMenuAsync(LoadGameSubMenu.GetScenePath());
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Arenbee.Actors.Players;
using Arenbee.GUI.Menus.Common;
using GameCore;
using GameCore.Actors;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Items;
using GameCore.SaveData;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Title;

[Tool]
public partial class MainSubMenu : OptionSubMenu
{
    public MainSubMenu()
    {
        PreventCancel = true;
        PreventCloseAll = true;
    }

    private OptionContainer _startOptions;
    public static string GetScenePath() => GDEx.GetScenePath();

    public override async Task TransitionOpenAsync()
    {
        var pos = _startOptions.Position;
        _startOptions.Position = new Vector2(pos.x, -_startOptions.Size.y);
        var tween = _startOptions.CreateTween();
        tween.TweenProperty(_startOptions, "position:y", pos.y, 0.4f);
        await ToSignal(tween, Signals.FinishedSignal);
    }

    protected override void OnItemSelected()
    {
        string titleChoice = CurrentContainer.CurrentItem.GetData<string>("value");
        if (titleChoice == null)
            return;
        switch (titleChoice)
        {
            case TitleMenuOptions.Continue:
                OpenContiueSaveSubMenu();
                break;
            case TitleMenuOptions.NewGame:
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

    private static List<TextOption> GetMenuOptions()
    {
        var textOptionScene = GD.Load<PackedScene>(TextOption.GetScenePath());
        var options = new List<TextOption>();
        var gameSaves = SaveService.GetGameSaves();
        foreach (var optionString in TitleMenuOptions.GetAll())
        {
            var option = textOptionScene.Instantiate<TextOption>();
            option.LabelText = optionString;
            option.OptionData["value"] = optionString;
            if (optionString == TitleMenuOptions.Continue && gameSaves.Count == 0)
                option.Disabled = true;
            options.Add(option);
        }
        return options;
    }

    private void StartNewGame()
    {
        Loading = true;
        static async Task Callback(Loader loader)
        {
            var sessionScene = loader.GetObject<PackedScene>(Locator.Root?.GameSessionScenePath);
            var adyScene = loader.GetObject<PackedScene>(Ady.GetScenePath());
            List<ActorBase> actors = new() { adyScene.Instantiate<Ady>() };
            List<ItemStack> items = new() { new ItemStack("HockeyStick", 1) };
            GameSave gameSave = new(actors, items);
            var session = sessionScene.Instantiate<GameSessionBase>();
            Locator.ProvideGameSession(session);
            Locator.Root?.GameSessionContainer.AddChild(session);
            session.Init(gameSave);
            await Locator.Root?.GUIController.CloseLayerAsync(new GUICloseRequest()
            {
                CloseRequestType = CloseRequestType.AllLayers,
                PreventAnimation = true
            });
        }

        var tController = Locator.TransitionController;
        var request = new TransitionRequest(
            BasicLoadingScreen.GetScenePath(),
            TransitionType.Game,
            FadeTransition.GetScenePath(),
            FadeTransition.GetScenePath(),
            new string[] { Locator.Root?.GameSessionScenePath, Ady.GetScenePath() },
            Callback);
        tController.RequestTransition(request);
    }

    private void OpenContiueSaveSubMenu()
    {
        GUIOpenRequest request = new(LoadGameSubMenu.GetScenePath());
        RequestOpenSubMenu(request);
    }

    private static class TitleMenuOptions
    {
        public static List<string> GetAll()
        {
            return new List<string>()
            {
                Continue,
                NewGame
            };
        }
        public const string Continue = "Continue";
        public const string NewGame = "New Game";
    }
}

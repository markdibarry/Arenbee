using System.Threading.Tasks;
using GameCore.Extensions;
using GameCore.Game.SaveData;
using GameCore.GUI;
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
        var tween = GetTree().CreateTween();
        var prop = tween.TweenProperty(_startOptions, "position:y", pos.y, 0.4f);
        await ToSignal(tween, "finished");
        tween.Dispose();
        prop.Dispose();
    }

    protected override void OnItemSelected()
    {
        base.OnItemSelected();
        var titleChoice = CurrentContainer.CurrentItem.GetData<string>("titleChoice");
        if (titleChoice == null)
            return;
        IsActive = false;
        switch (titleChoice)
        {
            case "Continue":
                ContinueSavedGame();
                break;
            case "NewGame":
                StartNewGame();
                break;
        }
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        _startOptions = OptionContainers.Find(x => x.Name == "MainOptions");
    }

    private void StartNewGame()
    {
        var closeRequest = new SubMenuCloseRequest(
            callback: () => Locator.Root?.StartGame(SaveService.GetNewGame())
        );
        RaiseRequestedClose(closeRequest);
    }

    private void ContinueSavedGame()
    {
        var closeRequest = new SubMenuCloseRequest(
            callback: () => Locator.Root?.StartGame(SaveService.LoadGame())
        );
        RaiseRequestedClose(closeRequest);
    }
}

using System.Collections.Generic;
using System.Linq;
using Arenbee.Actors;
using Arenbee.AreaScenes;
using Arenbee.GUI.Menus.PartyMenus;
using Arenbee.Items;
using GameCore;
using GameCore.GUI;
using GameCore.Input;
using GameCore.Utility;

namespace Arenbee;

public partial class GameSession : BaseGameSession
{
    public static string GetScenePath() => GDEx.GetScenePath();
    public Party? MainParty { get; set; }
    public List<Party> Parties { get; private set; } = new();
    public SessionState SessionState { get; private set; } = new();

    public override void _Ready()
    {
        SetNodeReferences();
    }

    public override void _Process(double delta)
    {
        bool paused = CurrentAreaScene == null || CurrentAreaScene.ProcessMode == ProcessModeEnum.Disabled;
        SessionState.Update(delta, paused);
    }

    public Party? GetParty(string id) => Parties.FirstOrDefault(x => x.Id == id);

    public override void HandleInput(IGUIInputHandler menuInput, double delta)
    {
        if (menuInput.Start.IsActionJustPressed && !GUIController.GUIActive)
            OpenPartyMenuAsync();
    }

    private void InitAreaScene()
    {
        // TODO: Make game
        if (CurrentAreaScene == null)
        {
            var demoAreaScene = GDEx.Instantiate<AreaScene>(Paths.DemoLevel1);
            AddAreaScene(demoAreaScene);
        }
    }

    private async void OpenPartyMenuAsync()
    {
        await GUIController.OpenMenuAsync(PartyMenu.GetScenePath());
    }

    public override void Init(GUIController guiController, IGameSave gameSave)
    {
        GUIController = guiController;
        GameSave save = (GameSave)gameSave;
        IEnumerable<Inventory> inventories = save.Inventories.Select(x => x.CreateInventory());
        Parties = save.Parties.Select(x => x.ToParty(inventories)).ToList();
        MainParty = GetParty(save.MainPartyId);
        SessionState = save.SessionState;

        InitAreaScene();
        ActorBody actorBody = InitMainActor();
        actorBody.GlobalPosition = CurrentAreaScene!.GetSpawnPoint(0);
        CurrentAreaScene!.AddActorBody(actorBody);
    }

    private ActorBody InitMainActor()
    {
        Actor actor = MainParty!.Actors.First();
        ActorBody actorBody = actor.CreateBody<ActorBody>();
        actorBody.SetRole((int)ActorRole.Player);
        Locator.Root.GameCamera.CurrentTarget = actorBody;
        actorBody.SetInputHandler(Locator.Root.PlayerOneInput);
        return actorBody;
    }
}

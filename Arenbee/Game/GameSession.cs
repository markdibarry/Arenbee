using System.Collections.Generic;
using System.Linq;
using Arenbee.Game;
using Arenbee.GUI.Menus;
using Arenbee.Items;
using Arenbee.SaveData;
using GameCore;
using GameCore.Actors;
using GameCore.AreaScenes;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Input;
using GameCore.Items;
using GameCore.SaveData;

namespace Arenbee;

public partial class GameSession : AGameSession
{
    public static string GetScenePath() => GDEx.GetScenePath();
    public PlayerParty Party { get; private set; }
    public SessionState SessionState { get; private set; } = new();

    public override void _Ready()
    {
        SetNodeReferences();
    }

    public override void _Process(double delta)
    {
        SessionState.Update(delta, CurrentAreaScene.ProcessMode == ProcessModeEnum.Disabled);
    }

    public override void HandleInput(GUIInputHandler menuInput, double delta)
    {
        if (menuInput.Start.IsActionJustPressed && !GUIController.GUIActive)
            OpenPartyMenuAsync();
    }

    private void InitAreaScene()
    {
        // TODO: Make game
        if (CurrentAreaScene == null)
        {
            var demoAreaScene = GDEx.Instantiate<AAreaScene>(Paths.DemoLevel1);
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
        IEnumerable<AActor> actors = save.ActorData.Select(x => x.CreateActor());
        List<AItemStack> itemStacks = new();
        foreach (ItemStackData itemStackData in save.Items)
        {
            AItemStack? itemStack = itemStackData.CreateItemStack();
            if (itemStack != null)
                itemStacks.Add(itemStack);
        }
        Party = new(actors, new Inventory(itemStacks));
        SessionState = save.SessionState;
        InitAreaScene();
    }
}

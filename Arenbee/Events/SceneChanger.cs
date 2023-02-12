using System.Collections.Generic;
using System.Linq;
using Arenbee.Game;
using Arenbee.GUI;
using GameCore;
using GameCore.Actors;
using GameCore.AreaScenes;
using GameCore.Events;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.Events;

public partial class SceneChanger : SceneChangerBase
{
    protected override void ChangeScene()
    {
        if (GameSession == null)
            return;

        TransitionRequest request = new(
            TransitionType.Session,
            FadeTransition.GetScenePath(),
            new string[] { PackedScenePath },
            async (loader) =>
            {
                var areaScenePacked = loader.GetObject<PackedScene>(PackedScenePath);
                await GUIController.CloseAllLayersAsync();
                PlayerParty party = ((GameSession)GameSession).Party;
                foreach (var player in party.Actors)
                    GameSession.CurrentAreaScene?.RemoveActor(player.ActorBody!);
                GameSession.RemoveAreaScene();

                AAreaScene areaScene = areaScenePacked.Instantiate<AAreaScene>();
                GameSession.AddAreaScene(areaScene);
                AActor actor = party.Actors.ElementAt(0);
                areaScene.AddActor(actor.ActorBody!, areaScene.GetSpawnPoint(0));
            });
        TController.RequestTransition(request);
    }
}

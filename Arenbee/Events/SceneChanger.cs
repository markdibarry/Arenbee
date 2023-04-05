using System.Linq;
using System.Threading.Tasks;
using Arenbee.Game;
using Arenbee.GUI;
using GameCore.Actors;
using GameCore.AreaScenes;
using GameCore.Events;
using GameCore.GUI;
using Godot;

namespace Arenbee.Events;

public partial class SceneChanger : SceneChangerBase
{
    protected override async Task ChangeScene()
    {
        if (GameSession == null)
            return;
        GameSession.Pause();
        TransitionRequest request = new(
            TransitionType.Session,
            FadeTransition.GetScenePath(),
            new string[] { PackedScenePath },
            async (loader) =>
            {
                var areaScenePacked = loader.GetObject<PackedScene>(PackedScenePath);
                await GUIController.CloseAllLayersAsync();
                Party? party = ((GameSession)GameSession).MainParty;
                foreach (var player in party!.Actors)
                    GameSession.CurrentAreaScene?.RemoveActorBody(player.ActorBody!);
                GameSession.RemoveAreaScene();

                AAreaScene areaScene = areaScenePacked!.Instantiate<AAreaScene>();
                GameSession.AddAreaScene(areaScene);
                AActor actor = party!.Actors.First();
                actor.ActorBody!.GlobalPosition = areaScene.GetSpawnPoint(0);
                areaScene.AddActorBody(actor.ActorBody!);
            });
        await TController.TransitionAsync(request);
        GameSession.Resume();
    }
}

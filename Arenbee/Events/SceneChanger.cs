using System.Threading.Tasks;
using Arenbee.GUI;
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
        var tController = Locator.TransitionController;
        var request = new TransitionRequest(
            //BasicLoadingScreen.GetScenePath(),
            TransitionType.Session,
            FadeTransition.GetScenePath(),
            //FadeTransition.GetScenePath(),
            new string[] { PackedScenePath },
            (loader) =>
            {
                var newAreaScene = loader.GetObject<PackedScene>(PackedScenePath);
                Locator.Root?.GUIController.CloseAll();
                Locator.Session?.RemoveAreaScene();
                Locator.Session?.AddAreaScene(newAreaScene.Instantiate<AreaScene>());
                return Task.CompletedTask;
            });
        tController.ChangeScene(request);
    }
}

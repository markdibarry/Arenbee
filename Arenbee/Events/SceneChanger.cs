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
            TransitionType.Session,
            FadeTransition.GetScenePath(),
            new string[] { PackedScenePath },
            async (loader) =>
            {
                var newAreaScene = loader.GetObject<PackedScene>(PackedScenePath);
                await Locator.Root?.GUIController.CloseAllLayersAsync();
                Locator.Session?.RemoveAreaScene();
                Locator.Session?.AddAreaScene(newAreaScene.Instantiate<AreaScene>());
            });
        tController.RequestTransition(request);
    }
}

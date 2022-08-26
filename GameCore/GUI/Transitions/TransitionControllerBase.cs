using System.Threading.Tasks;
using GameCore.Extensions;
using GameCore.Utility;

namespace GameCore.GUI
{
    public abstract class TransitionControllerBase
    {
        public bool IsActive { get; set; }

        public TransitionType TransitionType { get; set; }

        public async Task LoadScene(TransitionData transitionData)
        {
            if (IsActive)
                return;
            IsActive = true;
            var sceneTransition = GetSceneTransition(transitionData);
            ClearTransitions();
            await sceneTransition.TransitionFromScene();
            RemoveOldScene();
            sceneTransition.AddNewScene();
            await sceneTransition.TransitionToScene();
            IsActive = false;
        }

        private SceneTransition GetSceneTransition(TransitionData transitionData)
        {
            return transitionData.TransitionType switch
            {
                TransitionType.Room => new RoomTransition(transitionData),
                TransitionType.Area => new AreaTransition(transitionData),
                TransitionType.Game => new GameTransition(transitionData),
                _ => null,
            };
        }

        private void ClearTransitions()
        {
            Locator.Root?.Transition.QueueFreeAllChildren();
            Locator.Session?.Transition.QueueFreeAllChildren();
        }

        private void RemoveOldScene()
        {
            Locator.Root?.GUIController.CloseAll();
            Locator.Session?.RemoveAreaScene();
        }
    }

    public enum TransitionType
    {
        None,
        Room,
        Area,
        World,
        Game
    }
}

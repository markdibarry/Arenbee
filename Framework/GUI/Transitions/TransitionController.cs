using System.Threading.Tasks;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.Utility;

namespace Arenbee.Framework.GUI
{
    public class TransitionController
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
            GameRoot.Instance.Transition.QueueFreeAllChildren();
            Locator.GetGameSession()?.Transition.QueueFreeAllChildren();
        }

        private void RemoveOldScene()
        {
            GameRoot.Instance.GUIController.CloseAll();
            Locator.GetGameSession()?.RemoveAreaScene();
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
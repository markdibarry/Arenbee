using System.Threading.Tasks;

namespace GameCore.GUI
{
    public class SceneTransition
    {
        public string ScenePath { get; set; }

        public virtual void AddNewScene() { }

        public virtual Task TransitionFromScene()
        {
            return Task.CompletedTask;
        }

        public virtual Task TransitionToScene()
        {
            return Task.CompletedTask;
        }
    }
}
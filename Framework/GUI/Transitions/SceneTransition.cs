using System.Threading.Tasks;

namespace Arenbee.Framework.GUI
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
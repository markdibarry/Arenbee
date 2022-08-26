using System.Threading.Tasks;

namespace GameCore.GUI
{
    public interface ITransition
    {
        Task TransitionStart();
        Task TransitionEnd();
    }
}
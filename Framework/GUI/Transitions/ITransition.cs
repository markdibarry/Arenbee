using System.Threading.Tasks;

namespace Arenbee.Framework.GUI
{
    public interface ITransition
    {
        Task TransitionStart();
        Task TransitionEnd();
    }
}
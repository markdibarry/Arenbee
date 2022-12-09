using System.Threading.Tasks;

namespace GameCore.GUI;

public interface IGUIController
{
    Task CloseLayerAsync(bool preventAnimation = false, object data = null);
}

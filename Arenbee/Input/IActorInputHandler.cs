using GameCore.Input;

namespace Arenbee.Input;

public interface IActorInputHandler : IInputHandler
{
    IInputAction Jump { get; }
    IInputAction Attack { get; }
    IInputAction SubAction { get; }
    IInputAction Run { get; }
}

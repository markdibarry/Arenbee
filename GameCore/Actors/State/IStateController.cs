using GameCore.Items;

namespace GameCore.Actors;

public interface IStateController
{
    AirStateMachineBase AirStateMachine { get; }
    MoveStateMachineBase MoveStateMachine { get; }
    ActionStateMachineBase ActionStateMachine { get; }
    HealthStateMachineBase HealthStateMachine { get; }
    bool BaseActionDisabled { get; set; }
    void Init();
    bool IsBlocked(BlockedState stateType);
    bool PlayHealthAnimation(string animationName);
    bool PlayActionAnimation(string animationName, HoldItem holdItem);
    bool PlayAirAnimation(string animationName);
    bool PlayMoveAnimation(string animationName);
    bool PlayFallbackAnimation();
    void UpdateStates(double delta);
}

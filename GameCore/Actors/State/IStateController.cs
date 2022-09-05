namespace GameCore.Actors;

public interface IStateController
{
    AirStateMachineBase AirStateMachine { get; }
    MoveStateMachineBase MoveStateMachine { get; }
    ActionStateMachineBase ActionStateMachine { get; }
    HealthStateMachineBase HealthStateMachine { get; }
    void ResetActionStateMachine();
    void SwitchActionStateMachine(ActionStateMachineBase actionStateMachineBase);
    void Init();
    bool IsBlocked(BlockedState stateType);
    bool PlayHealthAnimation(string animationName);
    bool PlayActionAnimation(string animationName);
    bool PlayAirAnimation(string animationName);
    bool PlayMoveAnimation(string animationName);
    bool PlayFallbackAnimation();
    void UpdateStates(float delta);
}

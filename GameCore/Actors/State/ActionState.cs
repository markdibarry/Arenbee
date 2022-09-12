using GameCore.Items;

namespace GameCore.Actors;

public abstract class ActionState : ActorState<ActionState, ActionStateMachineBase>
{
    public HoldItem HoldItem => StateMachine.HoldItem;

    protected override void PlayAnimation(string animationName)
    {
        StateController.PlayAnimation(animationName, "Action", HoldItem);
    }
}

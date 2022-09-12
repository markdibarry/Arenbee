using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Input;
using GameCore.Items;
using Godot;

namespace GameCore.Actors;

public class StateControllerBase
{
    public StateControllerBase(
        ActorBase actor,
        MoveStateMachineBase moveStateMachine,
        AirStateMachineBase airStateMachine,
        HealthStateMachineBase healthStateMachine,
        ActionStateMachineBase actionStateMachine,
        Func<ActorBase, BehaviorTree> behaviorTreeDelegate = null)
    {
        _actor = actor;
        HealthStateMachine = healthStateMachine;
        AirStateMachine = airStateMachine;
        MoveStateMachine = moveStateMachine;
        ActionStateMachine = actionStateMachine;
        _stateDisplayController = new();
        _behaviorTreeDelegate = behaviorTreeDelegate;
    }

    private readonly ActorBase _actor;
    private bool _baseActionDisabled;
    private readonly StateDisplayController _stateDisplayController;
    private readonly Func<ActorBase, BehaviorTree> _behaviorTreeDelegate;
    public BehaviorTree BehaviorTree { get; set; }
    public AirStateMachineBase AirStateMachine { get; }
    public MoveStateMachineBase MoveStateMachine { get; }
    public HealthStateMachineBase HealthStateMachine { get; }
    public ActionStateMachineBase ActionStateMachine { get; }
    public AnimationPlayer ActorAnimationPlayer => _actor.AnimationPlayer;
    public List<HoldItem> HoldItems => _actor.HoldItemController.HoldItems;
    public bool BaseActionDisabled
    {
        get => _baseActionDisabled;
        set
        {
            if (_baseActionDisabled == value)
                return;
            _baseActionDisabled = value;
            if (_baseActionDisabled)
                ActionStateMachine?.ExitState();
            else
                ActionStateMachine?.Init();
        }
    }

    public void Init()
    {
        HealthStateMachine.Init();
        AirStateMachine.Init();
        MoveStateMachine.Init();
        ActionStateMachine.Init();
        BehaviorTree = _behaviorTreeDelegate?.Invoke(_actor);
        _stateDisplayController.CreateStateDisplay(_actor);
    }

    public bool IsBlocked(BlockedState stateType)
    {
        bool stateBlocked =
            HealthStateMachine.State.BlockedStates.HasFlag(stateType) ||
            MoveStateMachine.State.BlockedStates.HasFlag(stateType) ||
            AirStateMachine.State.BlockedStates.HasFlag(stateType);
        if (!BaseActionDisabled)
            stateBlocked |= ActionStateMachine.State.BlockedStates.HasFlag(stateType);
        foreach (var holdItem in HoldItems)
            stateBlocked |= holdItem.StateMachine.State.BlockedStates.HasFlag(stateType);
        return stateBlocked;
    }

    public bool PlayAnimation(string animationName, string stateMachineName, HoldItem holdItem = null)
    {
        if (!ValidateAnimation(stateMachineName))
            return false;
        string holdItemAnimationName = animationName;
        if (holdItem != null)
        {
            if (!holdItem.AnimationPlayer.HasAnimation(holdItemAnimationName))
                return false;
            animationName = $"{holdItem.HoldItemType}/{animationName}";
        }
        if (!ActorAnimationPlayer.HasAnimation(animationName))
            return false;
        foreach (var item in HoldItems)
        {
            if (item != holdItem && item.AnimationPlayer.CurrentAnimation != "RESET")
                item.AnimationPlayer.Play("RESET");
        }
        ActorAnimationPlayer.Play(animationName);
        holdItem?.AnimationPlayer.Play(holdItemAnimationName);
        return true;
    }

    public bool ValidateAnimation(string stateMachineName)
    {
        if (HealthStateMachine.State.AnimationName == null)
        {
            if (stateMachineName == "Action")
                return true;
            if (ActionStateMachine.State.AnimationName == null &&
                HoldItems.All(x => x.StateMachine.State.AnimationName == null))
            {
                if (stateMachineName == "Air")
                    return true;
                if (AirStateMachine.State.AnimationName == null)
                {
                    if (stateMachineName == "Move")
                        return true;
                }
            }
        }
        return false;
    }

    public bool PlayFallbackAnimation()
    {
        if (HealthStateMachine.State.AnimationName != null)
            return PlayAnimation(HealthStateMachine.State.AnimationName, "Health");
        foreach (var holdItem in HoldItems)
        {
            if (holdItem.StateMachine.State.AnimationName != null)
                return PlayAnimation(holdItem.StateMachine.State.AnimationName, "Action", holdItem);
        }
        if (ActionStateMachine.State.AnimationName != null)
            return PlayAnimation(ActionStateMachine.State.AnimationName, "Action", null);
        else if (AirStateMachine.State.AnimationName != null)
            return PlayAnimation(AirStateMachine.State.AnimationName, "Air");
        else if (MoveStateMachine.State.AnimationName != null)
            return PlayAnimation(MoveStateMachine.State.AnimationName, "Move");
        return false;
    }

    public void UpdateStates(double delta)
    {
        MoveStateMachine.Update(delta);
        AirStateMachine.Update(delta);
        HealthStateMachine.Update(delta);
        if (!BaseActionDisabled)
            ActionStateMachine.Update(delta);
        foreach (var holdItem in HoldItems)
            holdItem.StateMachine.Update(delta);
        _stateDisplayController.Update(this);
        BehaviorTree?.Update(delta);
    }
}

using System;
using System.Collections.Generic;
using GameCore.Input;
using GameCore.Items;
using Godot;

namespace GameCore.Actors;

public class StateController : IStateController
{
    public StateController(
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
        return HealthStateMachine.State.BlockedStates.HasFlag(stateType) ||
            MoveStateMachine.State.BlockedStates.HasFlag(stateType) ||
            ActionStateMachine.State.BlockedStates.HasFlag(stateType) ||
            AirStateMachine.State.BlockedStates.HasFlag(stateType);
    }

    public bool PlayHealthAnimation(string animationName)
    {
        if (!ActorAnimationPlayer.HasAnimation(animationName))
            return false;
        foreach (var holdItem in HoldItems)
        {
            if (holdItem.AnimationPlayer.CurrentAnimation != "RESET")
                holdItem.AnimationPlayer.Play("RESET");
        }
        ActorAnimationPlayer.Play(animationName);
        return true;
    }

    public bool PlayActionAnimation(string animationName, HoldItem holdItem)
    {
        if (HealthStateMachine.State.AnimationName != null)
            return false;
        string playerAnimPath = animationName;
        if (holdItem == null)
        {
            if (!ActorAnimationPlayer.HasAnimation(playerAnimPath))
                return false;
        }
        else
        {
            playerAnimPath = $"{holdItem.HoldItemType}/{animationName}";
            if (!ActorAnimationPlayer.HasAnimation(playerAnimPath)
                || !holdItem.AnimationPlayer.HasAnimation(animationName))
                return false;
            holdItem.AnimationPlayer.Play(animationName);
        }
        ActorAnimationPlayer.Play(playerAnimPath);
        return true;
    }

    public bool PlayAirAnimation(string animationName)
    {
        if (HealthStateMachine.State.AnimationName != null
            || ActionStateMachine.State.AnimationName != null)
            return false;
        if (!ActorAnimationPlayer.HasAnimation(animationName))
            return false;
        foreach (var holdItem in HoldItems)
        {
            if (holdItem.AnimationPlayer.CurrentAnimation != "RESET")
                holdItem.AnimationPlayer.Play("RESET");
        }
        ActorAnimationPlayer.Play(animationName);
        return true;
    }

    public bool PlayMoveAnimation(string animationName)
    {
        if (HealthStateMachine.State.AnimationName != null
            || ActionStateMachine.State.AnimationName != null
            || AirStateMachine.State.AnimationName != null)
            return false;
        if (!ActorAnimationPlayer.HasAnimation(animationName))
            return false;
        foreach (var holdItem in HoldItems)
        {
            if (holdItem.AnimationPlayer.CurrentAnimation != "RESET")
                holdItem.AnimationPlayer.Play("RESET");
        }
        ActorAnimationPlayer.Play(animationName);
        return true;
    }

    public bool PlayFallbackAnimation()
    {
        if (HealthStateMachine.State.AnimationName != null)
            return PlayHealthAnimation(HealthStateMachine.State.AnimationName);
        foreach (var holdItem in HoldItems)
        {
            if (holdItem.StateMachine.State.AnimationName != null)
                return PlayActionAnimation(holdItem.StateMachine.State.AnimationName, holdItem);
        }
        if (ActionStateMachine.State.AnimationName != null)
            return PlayActionAnimation(ActionStateMachine.State.AnimationName, null);
        else if (AirStateMachine.State.AnimationName != null)
            return PlayAirAnimation(AirStateMachine.State.AnimationName);
        else if (MoveStateMachine.State.AnimationName != null)
            return PlayMoveAnimation(MoveStateMachine.State.AnimationName);
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

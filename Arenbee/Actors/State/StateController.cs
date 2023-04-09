using System;
using System.Collections.Generic;
using System.Linq;
using Arenbee.Actors.Behavior;
using Arenbee.Input;
using Arenbee.Items;
using GameCore.Actors;
using Godot;

namespace Arenbee.Actors;

public class StateController : IStateController
{
    public StateController(
        ActorBody actorBody,
        MoveStateMachineBase moveStateMachine,
        AirStateMachineBase airStateMachine,
        HealthStateMachineBase healthStateMachine,
        ActionStateMachineBase actionStateMachine,
        Func<ActorBody, BehaviorTree>? behaviorTreeDelegate = null)
    {
        _actorBody = actorBody;
        HealthStateMachine = healthStateMachine;
        AirStateMachine = airStateMachine;
        MoveStateMachine = moveStateMachine;
        BaseActionStateMachine = actionStateMachine;
        _stateDisplayController = new();
        _behaviorTreeDelegate = behaviorTreeDelegate;
    }

    private readonly ActorBody _actorBody;
    private bool _baseActionDisabled;
    private readonly StateDisplayController _stateDisplayController;
    private readonly Func<ActorBody, BehaviorTree>? _behaviorTreeDelegate;
    public BehaviorTree? BehaviorTree { get; set; }
    public ActorBodyStateMachine AirStateMachine { get; }
    public ActorBodyStateMachine MoveStateMachine { get; }
    public ActorBodyStateMachine HealthStateMachine { get; }
    public ActorBodyStateMachine BaseActionStateMachine { get; }
    public AnimationPlayer ActorAnimationPlayer => _actorBody.AnimationPlayer;
    public List<HoldItem> HoldItems => _actorBody.HoldItemController.HoldItems;
    public bool BaseActionDisabled
    {
        get => _baseActionDisabled;
        set
        {
            if (_baseActionDisabled == value)
                return;
            _baseActionDisabled = value;
            if (_baseActionDisabled)
                BaseActionStateMachine.ExitState();
            else
                BaseActionStateMachine.Reset();
        }
    }

    public void Init()
    {
        HealthStateMachine.Reset();
        AirStateMachine.Reset();
        MoveStateMachine.Reset();
        BaseActionStateMachine.Reset();
        BehaviorTree = _behaviorTreeDelegate?.Invoke(_actorBody);
        _stateDisplayController.CreateStateDisplay(_actorBody);
    }

    public bool IsBlocked(BlockedState stateType)
    {
        bool stateBlocked = HealthStateMachine.IsBlocked(stateType)
            || MoveStateMachine.IsBlocked(stateType)
            || AirStateMachine.IsBlocked(stateType);
        if (!BaseActionDisabled)
            stateBlocked |= BaseActionStateMachine.IsBlocked(stateType);
        foreach (HoldItem holdItem in HoldItems)
            stateBlocked |= holdItem.StateMachine.IsBlocked(stateType);
        return stateBlocked;
    }

    public bool TryPlayAnimation(string animationName, string stateMachineName, HoldItem? holdItem = null)
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
        if (stateMachineName == "Health")
            return true;
        if (HealthStateMachine.State.AnimationName != string.Empty)
            return false;
        if (stateMachineName == "Action")
            return true;
        if ((BaseActionDisabled || BaseActionStateMachine.State.AnimationName == string.Empty) &&
            HoldItems.All(x => x.StateMachine.State.AnimationName == string.Empty))
        {
            if (stateMachineName == "Air")
                return true;
            if (AirStateMachine.State.AnimationName == string.Empty)
            {
                if (stateMachineName == "Move")
                    return true;
            }
        }
        return false;
    }

    public bool PlayFallbackAnimation()
    {
        if (HealthStateMachine.State.AnimationName != string.Empty)
            return TryPlayAnimation(HealthStateMachine.State.AnimationName, "Health");
        foreach (var holdItem in HoldItems)
        {
            if (holdItem.StateMachine.State.AnimationName != string.Empty)
                return TryPlayAnimation(holdItem.StateMachine.State.AnimationName, "Action", holdItem);
        }
        if (!BaseActionDisabled && BaseActionStateMachine.State.AnimationName != string.Empty)
            return TryPlayAnimation(BaseActionStateMachine.State.AnimationName, "Action", null);
        else if (AirStateMachine.State.AnimationName != string.Empty)
            return TryPlayAnimation(AirStateMachine.State.AnimationName, "Air");
        else if (MoveStateMachine.State.AnimationName != string.Empty)
            return TryPlayAnimation(MoveStateMachine.State.AnimationName, "Move");
        return false;
    }

    public void UpdateStates(double delta)
    {
        if (_actorBody.InputHandler is DummyInputHandler)
            BehaviorTree?.Update(delta);
        MoveStateMachine.Update(delta);
        AirStateMachine.Update(delta);
        HealthStateMachine.Update(delta);
        if (!BaseActionDisabled)
            BaseActionStateMachine.Update(delta);
        foreach (var holdItem in HoldItems)
            holdItem.StateMachine.Update(delta);
        _stateDisplayController.Update(this);
    }
}

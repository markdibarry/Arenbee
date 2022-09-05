using System;
using GameCore.Input;
using GameCore.Items;
using Godot;

namespace GameCore.Actors;

public class StateController : IStateController
{
    public StateController(
        Actor actor,
        MoveStateMachineBase moveStateMachineBase,
        AirStateMachineBase airStateMachineBase,
        HealthStateMachineBase healthStateMachineBase,
        Func<Actor, ActionStateMachineBase> actionStateMachineDelegate,
        Func<Actor, BehaviorTree> behaviorTreeDelegate = null)
    {
        _actor = actor;
        HealthStateMachine = healthStateMachineBase;
        AirStateMachine = airStateMachineBase;
        MoveStateMachine = moveStateMachineBase;
        _actionStateMachineDelegate = actionStateMachineDelegate;
        _actionStateMachine = actionStateMachineDelegate(actor);
        _stateDisplayController = new();
        _behaviorTreeDelegate = behaviorTreeDelegate;
    }

    private readonly Actor _actor;
    private readonly StateDisplayController _stateDisplayController;
    private readonly Func<Actor, ActionStateMachineBase> _actionStateMachineDelegate;
    private readonly Func<Actor, BehaviorTree> _behaviorTreeDelegate;
    private ActionStateMachineBase _actionStateMachine;
    public BehaviorTree BehaviorTree { get; set; }
    public AirStateMachineBase AirStateMachine { get; }
    public MoveStateMachineBase MoveStateMachine { get; }
    public ActionStateMachineBase ActionStateMachine { get => _actionStateMachine; }
    public HealthStateMachineBase HealthStateMachine { get; }
    public AnimationPlayer ActorAnimationPlayer => _actor.AnimationPlayer;
    public Weapon CurrentWeapon => _actor.WeaponSlot.CurrentWeapon;
    public AnimationPlayer WeaponAnimationPlayer => CurrentWeapon?.AnimationPlayer;

    public void ResetActionStateMachine()
    {
        SwitchActionStateMachine(_actionStateMachineDelegate(_actor));
        BehaviorTree = _behaviorTreeDelegate?.Invoke(_actor);
    }

    public void SwitchActionStateMachine(ActionStateMachineBase actionStateMachineBase)
    {
        ActionStateMachine?.ExitState();
        _actionStateMachine = actionStateMachineBase;
        ActionStateMachine.Init();
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
        if (CurrentWeapon != null && CurrentWeapon.AnimationPlayer.CurrentAnimation != "RESET")
            CurrentWeapon.AnimationPlayer.Play("RESET");
        ActorAnimationPlayer.Play(animationName);
        return true;
    }

    public bool PlayActionAnimation(string animationName)
    {
        if (HealthStateMachine.State.AnimationName != null)
            return false;
        string playerAnimPath = animationName;
        if (CurrentWeapon == null)
        {
            if (!ActorAnimationPlayer.HasAnimation(playerAnimPath))
                return false;
        }
        else
        {
            playerAnimPath = $"{CurrentWeapon.WeaponTypeName}/{animationName}";
            if (!ActorAnimationPlayer.HasAnimation(playerAnimPath)
                || !CurrentWeapon.AnimationPlayer.HasAnimation(animationName))
                return false;
            CurrentWeapon.AnimationPlayer.Play(animationName);
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
        if (CurrentWeapon != null && CurrentWeapon.AnimationPlayer.CurrentAnimation != "RESET")
            CurrentWeapon.AnimationPlayer.Play("RESET");
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
        if (CurrentWeapon != null && CurrentWeapon.AnimationPlayer.CurrentAnimation != "RESET")
            CurrentWeapon.AnimationPlayer.Play("RESET");
        ActorAnimationPlayer.Play(animationName);
        return true;
    }

    // public void PlaySubWeaponAttack(string animationName)
    // {
    // }

    public bool PlayFallbackAnimation()
    {
        if (HealthStateMachine.State.AnimationName != null)
            return PlayHealthAnimation(HealthStateMachine.State.AnimationName);
        if (ActionStateMachine.State.AnimationName != null)
            return PlayActionAnimation(ActionStateMachine.State.AnimationName);
        else if (AirStateMachine.State.AnimationName != null)
            return PlayAirAnimation(AirStateMachine.State.AnimationName);
        else if (MoveStateMachine.State.AnimationName != null)
            return PlayMoveAnimation(MoveStateMachine.State.AnimationName);
        return false;
    }

    public void UpdateStates(float delta)
    {
        MoveStateMachine.Update(delta);
        AirStateMachine.Update(delta);
        ActionStateMachine.Update(delta);
        HealthStateMachine.Update(delta);
        _stateDisplayController.Update(this);
        BehaviorTree?.Update(delta);
    }
}

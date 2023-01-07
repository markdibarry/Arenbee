﻿using GameCore.Actors;
using GameCore.Items;
using GameCore.Statistics;
using GameCore.Utility;

namespace Arenbee.Items;

public partial class HockeyStick : HoldItem
{
    public override void Init(ActorBase actor)
    {
        Setup("HockeyStick", GameCore.WeaponTypes.LongStick, actor, new ActionStateMachine(actor, this));
    }

    public HitBox WeakAttack1HitBox { get; set; }
    public HitBox WeakAttack2HitBox { get; set; }

    protected override void SetHitBoxes()
    {
        WeakAttack1HitBox.SetBasicMeleeBox(Actor);
        WeakAttack2HitBox.SetBasicMeleeBox(Actor);
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
        WeakAttack1HitBox = GetNode<HitBox>("WeakAttack1");
        WeakAttack2HitBox = GetNode<HitBox>("WeakAttack2");
    }
}

public class ActionStateMachine : ActionStateMachineBase
{
    public ActionStateMachine(ActorBase actor, HoldItem holdItem)
        : base(
            new ActionState[]
            {
                new NotAttacking(actor, holdItem),
                new WeakAttack1(actor, holdItem),
                new WeakAttack2(actor, holdItem)
            },
            actor, holdItem)
    {
    }

    protected class NotAttacking : ActionState
    {
        public NotAttacking(ActorBase actor, HoldItem? holdItem) : base(actor, holdItem)
        {
        }

        public override void Enter()
        {
            StateController.PlayFallbackAnimation();
        }

        public override void Update(double delta)
        {
        }

        public override void Exit() { }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (StateController.IsBlocked(BlockedState.Attack) || Actor.ContextAreas.Count > 0)
                return false;
            if (InputHandler.Attack.IsActionJustPressed)
                return stateMachine.TrySwitchTo<WeakAttack1>();
            return false;
        }
    }

    protected class WeakAttack1 : ActionState
    {
        public WeakAttack1(ActorBase actor, HoldItem? holdItem) : base(actor, holdItem)
        {
            AnimationName = "WeakAttack1";
        }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
        }

        public override void Update(double delta)
        {
        }

        public override void Exit()
        {
            var hockeyStick = HoldItem as HockeyStick;
            hockeyStick.WeakAttack1HitBox.SetMonitorableDeferred(false);
            hockeyStick.WeakAttack1HitBox.Visible = false;
        }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (StateController.IsBlocked(BlockedState.Attack)
                || HoldItem.AnimationPlayer.CurrentAnimation != AnimationName)
                return stateMachine.TrySwitchTo<NotAttacking>();
            if (InputHandler.Attack.IsActionJustPressed)
                return stateMachine.TrySwitchTo<WeakAttack2>();
            return false;
        }
    }

    protected class WeakAttack2 : ActionState
    {
        public WeakAttack2(ActorBase actor, HoldItem? holdItem) : base(actor, holdItem)
        {
            AnimationName = "WeakAttack2";
        }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
        }

        public override void Update(double delta)
        {
        }

        public override void Exit()
        {
            var hockeyStick = HoldItem as HockeyStick;
            hockeyStick.WeakAttack2HitBox.SetMonitorableDeferred(false);
            hockeyStick.WeakAttack2HitBox.Visible = false;
        }

        public override bool TrySwitch(IStateMachine stateMachine)
        {
            if (StateController.IsBlocked(BlockedState.Attack)
                || HoldItem.AnimationPlayer.CurrentAnimation != AnimationName)
                return stateMachine.TrySwitchTo<NotAttacking>();
            return false;
        }
    }
}

using GameCore.Actors;
using GameCore.Items;
using GameCore.Statistics;

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
        : base(actor, holdItem)
    {
        AddState<NotAttacking>();
        AddState<WeakAttack1>();
        AddState<WeakAttack2>();
        InitStates(this);
    }

    protected class NotAttacking : ActionState
    {
        public override void Enter()
        {
            StateController.PlayFallbackAnimation();
        }

        public override ActionState Update(double delta)
        {
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActionState CheckForTransitions()
        {
            if (StateController.IsBlocked(BlockedState.Attack) || Actor.ContextAreasActive > 0)
                return null;
            if (InputHandler.Attack.IsActionJustPressed)
                return GetState<WeakAttack1>();
            return null;
        }
    }

    protected class WeakAttack1 : ActionState
    {
        public WeakAttack1() { AnimationName = "WeakAttack1"; }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
        }

        public override ActionState Update(double delta)
        {
            return CheckForTransitions();
        }

        public override void Exit()
        {
            var hockeyStick = HoldItem as HockeyStick;
            hockeyStick.WeakAttack1HitBox.SetMonitorableDeferred(false);
            hockeyStick.WeakAttack1HitBox.Visible = false;
        }

        public override ActionState CheckForTransitions()
        {
            if (StateController.IsBlocked(BlockedState.Attack)
                || HoldItem.AnimationPlayer.CurrentAnimation != AnimationName)
                return GetState<NotAttacking>();
            if (InputHandler.Attack.IsActionJustPressed)
                return GetState<WeakAttack2>();
            return null;
        }
    }

    protected class WeakAttack2 : ActionState
    {
        public WeakAttack2() { AnimationName = "WeakAttack2"; }

        public override void Enter()
        {
            PlayAnimation(AnimationName);
        }

        public override ActionState Update(double delta)
        {
            return CheckForTransitions();
        }

        public override void Exit()
        {
            var hockeyStick = HoldItem as HockeyStick;
            hockeyStick.WeakAttack2HitBox.SetMonitorableDeferred(false);
            hockeyStick.WeakAttack2HitBox.Visible = false;
        }

        public override ActionState CheckForTransitions()
        {
            if (StateController.IsBlocked(BlockedState.Attack)
                || HoldItem.AnimationPlayer.CurrentAnimation != AnimationName)
                return GetState<NotAttacking>();
            return null;
        }
    }
}

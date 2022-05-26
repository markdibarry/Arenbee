using Arenbee.Framework.Actors;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Items;
using Arenbee.Framework.Statistics;

namespace Arenbee.Assets.Items
{
    public partial class HockeyStick : Weapon
    {
        public HockeyStick()
        {
            ItemId = "HockeyStick";
            WeaponTypeName = WeaponTypeConstants.LongStick;
        }

        public HitBox WeakAttack1HitBox { get; set; }
        public HitBox WeakAttack2HitBox { get; set; }

        public override ActionStateMachineBase GetActionStateMachine() => new ActionStateMachine(Holder);

        protected override void SetHitBoxes()
        {
            WeakAttack1HitBox.SetBasicMeleeBox(Holder);
            WeakAttack2HitBox.SetBasicMeleeBox(Holder);
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
        public ActionStateMachine(Actor actor)
            : base(actor)
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

            public override ActionState Update(float delta)
            {
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override ActionState CheckForTransitions()
            {
                if (StateController.IsBlocked(BlockableState.Attack) || Actor.ContextAreasActive > 0)
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

            public override ActionState Update(float delta)
            {
                return CheckForTransitions();
            }

            public override void Exit()
            {
                var hockeyStick = Weapon as HockeyStick;
                hockeyStick.WeakAttack1HitBox.SetMonitorableDeferred(false);
                hockeyStick.WeakAttack1HitBox.Visible = false;
            }

            public override ActionState CheckForTransitions()
            {
                if (StateController.IsBlocked(BlockableState.Attack)
                    || Weapon.AnimationPlayer.CurrentAnimation != AnimationName)
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

            public override ActionState Update(float delta)
            {
                return CheckForTransitions();
            }

            public override void Exit()
            {
                var hockeyStick = Weapon as HockeyStick;
                hockeyStick.WeakAttack2HitBox.SetMonitorableDeferred(false);
                hockeyStick.WeakAttack2HitBox.Visible = false;
            }

            public override ActionState CheckForTransitions()
            {
                if (StateController.IsBlocked(BlockableState.Attack)
                    || Weapon.AnimationPlayer.CurrentAnimation != AnimationName)
                    return GetState<NotAttacking>();
                return null;
            }
        }
    }
}

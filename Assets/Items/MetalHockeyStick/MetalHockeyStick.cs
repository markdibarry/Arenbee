using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Constants;

namespace Arenbee.Assets.Items
{
    public partial class MetalHockeyStick : HockeyStick
    {
        public override void _Ready()
        {
            base._Ready();
            ItemId = "MetalHockeyStick";
            WeaponTypeName = WeaponTypeConstants.LongStick;
            InitialState = new NotAttacking();
        }

        private class NotAttacking : ActorState
        {
            public NotAttacking() { IsInitialState = true; }

            public override void Enter()
            {
                StateController.PlayFallbackAnimation();
            }

            public override ActorState Update(float delta)
            {
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override ActorState CheckForTransitions()
            {
                if (!Actor.IsAttackDisabled && InputHandler.Attack.IsActionJustPressed)
                    return new WeakAttack1();
                return null;
            }
        }

        private class WeakAttack1 : ActorState
        {
            public WeakAttack1() { AnimationName = "WeakAttack1"; }
            private bool _canRetrigger;
            private float _retriggerTimer;

            public override void Enter()
            {
                PlayAnimation(AnimationName);
            }

            public override ActorState Update(float delta)
            {
                if (_retriggerTimer > 0)
                    _retriggerTimer -= delta;
                else
                    _canRetrigger = true;
                return CheckForTransitions();
            }

            public override void Exit()
            {
                Actor.WeaponSlot.CurrentWeapon.DisableHitBoxes(1);
            }

            public override ActorState CheckForTransitions()
            {
                if (Weapon.AnimationPlayer.CurrentAnimation != AnimationName)
                    return new NotAttacking();
                if (!Actor.IsAttackDisabled && InputHandler.Attack.IsActionJustPressed && _canRetrigger)
                    return new WeakAttack2();
                return null;
            }
        }

        private class WeakAttack2 : ActorState
        {
            public WeakAttack2() { AnimationName = "WeakAttack2"; }

            public override void Enter()
            {
                PlayAnimation(AnimationName);
            }

            public override ActorState Update(float delta)
            {
                return CheckForTransitions();
            }

            public override void Exit()
            {
                Actor.WeaponSlot.CurrentWeapon.DisableHitBoxes(2);
            }

            public override ActorState CheckForTransitions()
            {
                if (Weapon.AnimationPlayer.CurrentAnimation != AnimationName)
                    return new NotAttacking();
                return null;
            }
        }
    }
}

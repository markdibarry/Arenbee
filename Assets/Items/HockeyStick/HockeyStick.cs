using Arenbee.Assets.Actors.Default.State;
using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Enums;
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

        public override void InitActionState()
        {
            Holder.StateController.ActionStateMachine.TransitionTo<NotAttacking>();
        }

        public override void DisableHitBoxes(int hitboxNum)
        {
            switch (hitboxNum)
            {
                case 1:
                    WeakAttack1HitBox.SetDeferred("monitorable", false);
                    WeakAttack1HitBox.Visible = false;
                    break;
                case 2:
                    WeakAttack2HitBox.SetDeferred("monitorable", false);
                    WeakAttack2HitBox.Visible = false;
                    break;
            }
        }

        protected override void SetHitBoxes()
        {
            WeakAttack1HitBox.ActionData = new ActionData()
            {
                SourceName = Holder.Name,
                ActionType = ActionType.Melee,
                ElementDamage = Holder.Stats.ElementOffs.CurrentElement,
                StatusEffects = Holder.Stats.StatusEffectOffs.GetModifiers(),
                Value = Holder.Stats.Attributes.GetStat(AttributeType.Attack).ModifiedValue
            };

            WeakAttack2HitBox.ActionData = new ActionData()
            {
                SourceName = Holder.Name,
                ActionType = ActionType.Melee,
                ElementDamage = Holder.Stats.ElementOffs.CurrentElement,
                StatusEffects = Holder.Stats.StatusEffectOffs.GetModifiers(),
                Value = Holder.Stats.Attributes.GetStat(AttributeType.Attack).ModifiedValue
            };
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            WeakAttack1HitBox = GetNode<HitBox>("WeakAttack1");
            WeakAttack2HitBox = GetNode<HitBox>("WeakAttack2");
        }

        protected class NotAttacking : NotAttackingState
        {
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
                var result = CheckForBaseTransitions(out bool returnEarly);
                if (returnEarly)
                    return result;
                if (InputHandler.Attack.IsActionJustPressed)
                    return GetState<WeakAttack1>();
                return null;
            }
        }

        protected class WeakAttack1 : AttackingState
        {
            public WeakAttack1() { AnimationName = "WeakAttack1"; }

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
                Actor.WeaponSlot.CurrentWeapon.DisableHitBoxes(1);
            }

            public override ActorState CheckForTransitions()
            {
                var result = CheckForBaseTransitions(out bool returnEarly);
                if (returnEarly)
                    return result;
                if (Weapon.AnimationPlayer.CurrentAnimation != AnimationName)
                    return GetState<NotAttacking>();
                if (InputHandler.Attack.IsActionJustPressed)
                    return GetState<WeakAttack2>();
                return null;
            }
        }

        protected class WeakAttack2 : AttackingState
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
                var result = CheckForBaseTransitions(out bool returnEarly);
                if (returnEarly)
                    return result;
                if (Weapon.AnimationPlayer.CurrentAnimation != AnimationName)
                    return GetState<NotAttacking>();
                return null;
            }
        }
    }
}

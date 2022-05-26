using Arenbee.Assets.Projectiles;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Items;
using Arenbee.Framework.Statistics;
using Godot;

namespace Arenbee.Assets.Items
{
    public partial class Wand : Weapon
    {
        public Wand()
        {
            ItemId = "Wand";
            WeaponTypeName = WeaponTypeConstants.Wand;
        }

        public override ActionStateMachineBase GetActionStateMachine() => new ActionStateMachine(Holder);

        protected override void SetHitBoxes()
        {
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
        }

        public class ActionStateMachine : ActionStateMachineBase
        {
            public ActionStateMachine(Actor actor)
                : base(actor)
            {
                AddState<NotAttacking>();
                AddState<WeakAttack1>();
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
                    Fireball.CreateFireball(Actor);
                }

                public override ActionState Update(float delta)
                {
                    return CheckForTransitions();
                }

                public override void Exit()
                {
                }

                public override ActionState CheckForTransitions()
                {
                    if (StateController.IsBlocked(BlockableState.Attack)
                        || Weapon.AnimationPlayer.CurrentAnimation != AnimationName
                        || !InputHandler.Attack.IsActionPressed)
                        return GetState<NotAttacking>();
                    return null;
                }
            }
        }
    }
}

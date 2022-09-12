using Arenbee.Projectiles;
using GameCore.Actors;
using GameCore.Constants;
using GameCore.Items;

namespace Arenbee.Items;

public partial class Wand : HoldItem
{
    public override void Init(ActorBase actor)
    {
        Setup("Wand", WeaponTypeConstants.Wand, actor, new ActionStateMachine(actor, this));
    }

    protected override void SetHitBoxes()
    {
    }

    protected override void SetNodeReferences()
    {
        base.SetNodeReferences();
    }

    public class ActionStateMachine : ActionStateMachineBase
    {
        public ActionStateMachine(ActorBase actor, HoldItem holdItem)
            : base(actor, holdItem)
        {
            AddState<NotAttacking>();
            AddState<WeakAttack1>();
            AddState<Charge>();
            AddState<BigAttack1>();
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
                if (InputHandler.SubAction.IsActionJustPressed)
                    return GetState<WeakAttack1>();
                return null;
            }
        }

        protected class WeakAttack1 : ActionState
        {
            public WeakAttack1()
            {
                AnimationName = "WeakAttack1";
                BlockedStates = BlockedState.Jumping | BlockedState.Move;
            }
            private double _counter;
            private readonly double _countTime = 0.5;
            public override void Enter()
            {
                _counter = _countTime;
                PlayAnimation(AnimationName);
                Fireball.CreateFireball(Actor);
            }

            public override ActionState Update(double delta)
            {
                if (_counter > 0)
                    _counter -= delta;
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override ActionState CheckForTransitions()
            {
                if (StateController.IsBlocked(BlockedState.Attack)
                    || HoldItem.AnimationPlayer.CurrentAnimation != AnimationName
                    || !InputHandler.SubAction.IsActionPressed)
                    return GetState<NotAttacking>();
                if (_counter <= 0)
                    return GetState<Charge>();
                return null;
            }
        }

        protected class Charge : ActionState
        {
            public Charge()
            {
                AnimationName = "Charge";
                BlockedStates = BlockedState.Jumping | BlockedState.Move;
            }
            private double _counter;
            private readonly double _countTime = 1.2;
            public override void Enter()
            {
                _counter = _countTime;
                Actor.ShaderCycleStart = 2;
                Actor.ShaderCycleEnd = 4;
                Actor.ShaderSpeed = 1;
                PlayAnimation(AnimationName);
            }

            public override ActionState Update(double delta)
            {
                if (_counter > 0)
                {
                    _counter -= delta;
                    if (_counter <= 0)
                    {
                        Actor.ShaderSpeed = 1.5f;
                        Actor.ShaderCycleStart = 1;
                    }
                }

                return CheckForTransitions();
            }

            public override void Exit()
            {
                Actor.ShaderCycleStart = 0;
                Actor.ShaderCycleEnd = 0;
                Actor.ShaderSpeed = 0;
            }

            public override ActionState CheckForTransitions()
            {
                if (StateController.IsBlocked(BlockedState.Attack)
                    || HoldItem.AnimationPlayer.CurrentAnimation != AnimationName)
                    return GetState<NotAttacking>();
                if (!InputHandler.SubAction.IsActionPressed)
                {
                    if (_counter <= 0)
                        return GetState<BigAttack1>();
                    else
                        return GetState<WeakAttack1>();
                }

                return null;
            }
        }

        protected class BigAttack1 : ActionState
        {
            public BigAttack1() { AnimationName = "BigAttack1"; }
            public override void Enter()
            {
                PlayAnimation(AnimationName);
                FireballBig.CreateFireball(Actor);
            }

            public override ActionState Update(double delta)
            {
                return CheckForTransitions();
            }

            public override void Exit()
            {
            }

            public override ActionState CheckForTransitions()
            {
                if (StateController.IsBlocked(BlockedState.Attack)
                    || HoldItem.AnimationPlayer.CurrentAnimation != AnimationName
                    || !InputHandler.SubAction.IsActionPressed)
                    return GetState<NotAttacking>();
                return null;
            }
        }
    }
}

using Arenbee.Projectiles;
using GameCore;
using GameCore.Actors;
using GameCore.Items;
using GameCore.Utility;

namespace Arenbee.Items;

public partial class Wand : HoldItem
{
    public override void Init(ActorBase actor)
    {
        Setup("Wand", WeaponTypes.Wand, actor, new ActionStateMachine(actor, this));
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
            : base(
                new ActionState[]
                {
                    new NotAttacking(actor, holdItem),
                    new WeakAttack1(actor, holdItem),
                    new Charge(actor, holdItem),
                    new BigAttack1(actor, holdItem)
                },
                actor, holdItem)
        {
        }

        protected class NotAttacking : ActionState
        {
            public NotAttacking(ActorBase actor, HoldItem holdItem)
                : base(actor, holdItem)
            { }

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
                if (InputHandler.SubAction.IsActionJustPressed)
                    return stateMachine.TrySwitchTo<WeakAttack1>();
                return false;
            }
        }

        protected class WeakAttack1 : ActionState
        {
            public WeakAttack1(ActorBase actor, HoldItem holdItem)
                : base(actor, holdItem)
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

            public override void Update(double delta)
            {
                if (_counter > 0)
                    _counter -= delta;
            }

            public override void Exit() { }

            public override bool TrySwitch(IStateMachine stateMachine)
            {
                if (StateController.IsBlocked(BlockedState.Attack)
                    || HoldItem.AnimationPlayer.CurrentAnimation != AnimationName
                    || !InputHandler.SubAction.IsActionPressed)
                    return stateMachine.TrySwitchTo<NotAttacking>();
                if (_counter <= 0)
                    return stateMachine.TrySwitchTo<Charge>();
                return false;
            }
        }

        protected class Charge : ActionState
        {
            public Charge(ActorBase actor, HoldItem holdItem)
                : base(actor, holdItem)
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

            public override void Update(double delta)
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
            }

            public override void Exit()
            {
                Actor.ShaderCycleStart = 0;
                Actor.ShaderCycleEnd = 0;
                Actor.ShaderSpeed = 0;
            }

            public override bool TrySwitch(IStateMachine stateMachine)
            {
                if (StateController.IsBlocked(BlockedState.Attack)
                    || HoldItem.AnimationPlayer.CurrentAnimation != AnimationName)
                    return stateMachine.TrySwitchTo<NotAttacking>();
                if (!InputHandler.SubAction.IsActionPressed)
                {
                    if (_counter <= 0)
                        return stateMachine.TrySwitchTo<BigAttack1>();
                    else
                        return stateMachine.TrySwitchTo<WeakAttack1>();
                }

                return false;
            }
        }

        protected class BigAttack1 : ActionState
        {
            public BigAttack1(ActorBase actor, HoldItem holdItem)
                : base(actor, holdItem)
            {
                AnimationName = "BigAttack1";
            }

            public override void Enter()
            {
                PlayAnimation(AnimationName);
                FireballBig.CreateFireball(Actor);
            }

            public override void Update(double delta)
            {
            }

            public override void Exit()
            {
            }

            public override bool TrySwitch(IStateMachine stateMachine)
            {
                if (StateController.IsBlocked(BlockedState.Attack)
                    || HoldItem.AnimationPlayer.CurrentAnimation != AnimationName
                    || !InputHandler.SubAction.IsActionPressed)
                    return stateMachine.TrySwitchTo<NotAttacking>();
                return false;
            }
        }
    }
}

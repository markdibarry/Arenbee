using Arenbee.Actors;
using Arenbee.Projectiles;
using GameCore;
using GameCore.Actors;

namespace Arenbee.Items;

public partial class Wand : HoldItem
{
    public override void Init(ActorBody actorBody)
    {
        Setup(ItemIds.Wand, WeaponTypes.Wand, actorBody, new ActionStateMachine(actorBody, this));
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
        public ActionStateMachine(ActorBody actorBody, HoldItem holdItem)
            : base(
                new ActionState[]
                {
                    new NotAttacking(actorBody, holdItem),
                    new WeakAttack1(actorBody, holdItem),
                    new Charge(actorBody, holdItem),
                    new BigAttack1(actorBody, holdItem)
                },
                actorBody, holdItem)
        {
        }

        protected class NotAttacking : ActionState
        {
            public NotAttacking(ActorBody actorBody, HoldItem holdItem)
                : base(actorBody, holdItem)
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
                if (StateController.IsBlocked(BlockedState.Attack) || ActorBody.ContextAreas.Count > 0)
                    return false;
                if (InputHandler.SubAction.IsActionJustPressed)
                    return stateMachine.TrySwitchTo<WeakAttack1>();
                return false;
            }
        }

        protected class WeakAttack1 : ActionState
        {
            public WeakAttack1(ActorBody actorBody, HoldItem holdItem)
                : base(actorBody, holdItem)
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
                Fireball.CreateFireball(ActorBody);
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
                    || HoldItem!.AnimationPlayer.CurrentAnimation != AnimationName
                    || !InputHandler.SubAction.IsActionPressed)
                    return stateMachine.TrySwitchTo<NotAttacking>();
                if (_counter <= 0)
                    return stateMachine.TrySwitchTo<Charge>();
                return false;
            }
        }

        protected class Charge : ActionState
        {
            public Charge(ActorBody actorBody, HoldItem holdItem)
                : base(actorBody, holdItem)
            {
                AnimationName = "Charge";
                BlockedStates = BlockedState.Jumping | BlockedState.Move;
            }

            private double _counter;
            private readonly double _countTime = 1.2;
            public override void Enter()
            {
                _counter = _countTime;
                ActorBody.ShaderCycleStart = 2;
                ActorBody.ShaderCycleEnd = 4;
                ActorBody.ShaderSpeed = 1;
                PlayAnimation(AnimationName);
            }

            public override void Update(double delta)
            {
                if (_counter > 0)
                {
                    _counter -= delta;
                    if (_counter <= 0)
                    {
                        ActorBody.ShaderSpeed = 1.5f;
                        ActorBody.ShaderCycleStart = 1;
                    }
                }
            }

            public override void Exit()
            {
                ActorBody.ShaderCycleStart = 0;
                ActorBody.ShaderCycleEnd = 0;
                ActorBody.ShaderSpeed = 0;
            }

            public override bool TrySwitch(IStateMachine stateMachine)
            {
                if (StateController.IsBlocked(BlockedState.Attack)
                    || HoldItem!.AnimationPlayer.CurrentAnimation != AnimationName)
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
            public BigAttack1(ActorBody actorBody, HoldItem holdItem)
                : base(actorBody, holdItem)
            {
                AnimationName = "BigAttack1";
            }

            public override void Enter()
            {
                PlayAnimation(AnimationName);
                FireballBig.CreateFireball(ActorBody);
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
                    || HoldItem!.AnimationPlayer.CurrentAnimation != AnimationName
                    || !InputHandler.SubAction.IsActionPressed)
                    return stateMachine.TrySwitchTo<NotAttacking>();
                return false;
            }
        }
    }
}

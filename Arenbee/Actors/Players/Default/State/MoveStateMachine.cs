using GameCore.Actors;
using GameCore.Statistics;

namespace Arenbee.Actors.Default.State
{
    public class MoveStateMachine : MoveStateMachineBase
    {
        public MoveStateMachine(Actor actor)
            : base(actor)
        {
            AddState<Standing>();
            AddState<Walking>();
            AddState<Running>();
            InitStates(this);
        }

        public class Standing : MoveState
        {
            public Standing() { AnimationName = "Standing"; }
            public override void Enter()
            {
                PlayAnimation(AnimationName);
            }

            public override MoveState Update(float delta)
            {
                return CheckForTransitions();
            }

            public override MoveState CheckForTransitions()
            {
                if (StateController.IsBlocked(BlockedState.Move))
                    return null;
                if (Actor.Stats.StatusEffects.HasEffect(StatusEffectType.Burn))
                    return GetState<Running>();
                if (InputHandler.GetLeftAxis().x == 0)
                    return null;
                if (InputHandler.Run.IsActionPressed)
                    return GetState<Running>();
                return GetState<Walking>();
            }
        }

        public class Walking : MoveState
        {
            public Walking() { AnimationName = "Walk"; }
            public override void Enter()
            {
                PlayAnimation(AnimationName);
                Actor.MaxSpeed = Actor.WalkSpeed;
            }

            public override MoveState Update(float delta)
            {
                Actor.UpdateDirection();
                Actor.Move();
                return CheckForTransitions();
            }

            public override MoveState CheckForTransitions()
            {
                if (StateController.IsBlocked(BlockedState.Move))
                    return GetState<Standing>();
                if (Actor.Stats.StatusEffects.HasEffect(StatusEffectType.Burn))
                    return GetState<Running>();
                if (InputHandler.GetLeftAxis().x == 0)
                    return GetState<Standing>();
                if (InputHandler.Run.IsActionPressed)
                    return GetState<Running>();
                return null;
            }
        }

        public class Running : MoveState
        {
            public Running() { AnimationName = "Run"; }

            public override void Enter()
            {
                PlayAnimation(AnimationName);
                Actor.MaxSpeed = Actor.RunSpeed;
            }

            public override MoveState Update(float delta)
            {
                Actor.UpdateDirection();
                Actor.Move();
                return CheckForTransitions();
            }

            public override MoveState CheckForTransitions()
            {
                if (StateController.IsBlocked(BlockedState.Move))
                    return GetState<Standing>();
                if (Actor.Stats.HasEffect(StatusEffectType.Burn))
                    return null;
                if (InputHandler.GetLeftAxis().x == 0)
                    return GetState<Standing>();
                if (InputHandler.Run.IsActionPressed)
                    return null;
                return GetState<Walking>();
            }
        }
    }
}
using Arenbee.Framework.Actors;
using Arenbee.Framework.Statistics;

namespace Arenbee.Assets.Actors.Enemies.Default.State
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
            public override void Enter() { }

            public override MoveState Update(float delta)
            {
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override MoveState CheckForTransitions()
            {
                if (StateController.IsBlocked(BlockableState.Move))
                    return null;
                if (Actor.Stats.HasEffect(StatusEffectType.Burn))
                    return GetState<Running>();
                if (Actor.InputHandler.GetLeftAxis().x != 0)
                    return GetState<Walking>();
                return null;
            }
        }

        public class Walking : MoveState
        {
            public override void Enter()
            {
                Actor.MaxSpeed = Actor.WalkSpeed;
            }

            public override MoveState Update(float delta)
            {
                Actor.UpdateDirection();
                Actor.Move();
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override MoveState CheckForTransitions()
            {
                if (StateController.IsBlocked(BlockableState.Move))
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
            public override void Enter()
            {
                Actor.MaxSpeed = Actor.RunSpeed;
            }

            public override MoveState Update(float delta)
            {
                Actor.UpdateDirection();
                Actor.Move();
                return CheckForTransitions();
            }

            public override void Exit() { }

            public override MoveState CheckForTransitions()
            {
                if (StateController.IsBlocked(BlockableState.Move))
                    return GetState<Standing>();
                if (Actor.Stats.HasEffect(StatusEffectType.Burn))
                    return null;
                if (InputHandler.GetLeftAxis().x == 0)
                    return GetState<Standing>();
                if (!InputHandler.Run.IsActionPressed)
                    return GetState<Walking>();
                return null;
            }
        }
    }
}
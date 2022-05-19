using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies
{
    public partial class Plant : Actor
    {
        public class MoveStateMachine : MoveStateMachineBase
        {
            public MoveStateMachine(Actor actor)
                : base(actor)
            {
                AddState<Standing>();
                InitStates(this);
            }

            public class NotAttacking : ActionState
            {
                public override void Enter() { }

                public override ActionState Update(float delta)
                {
                    return CheckForTransitions();
                }

                public override void Exit() { }

                public override ActionState CheckForTransitions()
                {
                    return null;
                }
            }

            private class Standing : MoveState
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

                public override void Exit() { }

                public override MoveState CheckForTransitions()
                {
                    return null;
                }
            }
        }
    }
}

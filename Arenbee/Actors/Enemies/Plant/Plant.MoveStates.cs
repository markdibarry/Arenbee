using GameCore.Actors;

namespace Arenbee.Actors.Enemies;

public partial class Plant : Actor
{
    public class MoveStateMachine : MoveStateMachineBase
    {
        public MoveStateMachine(ActorBase actor)
            : base(
                new MoveState[]
                {
                    new Standing(actor)
                },
                actor)
        {
        }

        private class Standing : MoveState
        {
            public Standing(ActorBase actor) : base(actor)
            {
                AnimationName = "Standing";
            }
            public override void Enter()
            {
                PlayAnimation(AnimationName);
            }

            public override void Update(double delta)
            {
            }

            public override void Exit() { }
        }
    }
}

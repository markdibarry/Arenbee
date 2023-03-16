using GameCore.Actors;

namespace Arenbee.Actors.Enemies;

public partial class Plant : ActorBody
{
    public class MoveStateMachine : MoveStateMachineBase
    {
        public MoveStateMachine(ActorBody actor)
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
            public Standing(ActorBody actor) : base(actor)
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

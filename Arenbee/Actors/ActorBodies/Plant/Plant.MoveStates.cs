namespace Arenbee.Actors.ActorBodies;

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
        }
    }
}

using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Players.BaseStates
{
    public class Idle : ActorState
    {
        public Idle() { AnimationName = "Idle"; }
        public override void Enter()
        {
            PlayAnimation(AnimationName);
        }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            if (Actor.IsRunStuck > 0)
                return new Run();

            if (Actor.ShouldWalk())
            {
                if (Actor.ShouldRun())
                    return new Run();
                return new Walk();
            }
            return null;
        }
    }
}
using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.BaseStates
{
    public class Idle : ActorState
    {
        public override void Enter() { }

        public override ActorState Update(float delta)
        {
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            if (Actor.IsRunStuck > 0)
                return new Run();
            if (!Actor.IsWalkDisabled && Actor.InputHandler.GetLeftAxis().x != 0)
                return new Walk();
            return null;
        }
    }
}
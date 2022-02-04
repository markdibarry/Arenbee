using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Items.HockeyStickNS
{
    public class NotAttacking : State<Actor>
    {
        public NotAttacking() { IsInitialState = true; }

        public override void Enter()
        {
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
            if (InputHandler.Attack.IsActionJustPressed && !Actor.IsAttackDisabled)
            {
                StateMachine.TransitionTo(new WeakAttack1());
            }
        }
    }
}
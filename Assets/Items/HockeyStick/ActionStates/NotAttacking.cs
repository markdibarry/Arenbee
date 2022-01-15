using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Constants;
using Godot;

namespace Arenbee.Assets.Items.HockeyStickNS
{
    public class NotAttacking : State<Actor>
    {
        public NotAttacking() { IsInitialState = true; }

        public override void Enter()
        {
        }

        public override void Update()
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
            if (Input.IsActionJustPressed(ActionConstants.Attack))
            {
                StateMachine.TransitionTo(new WeakAttack1());
            }
        }

    }
}
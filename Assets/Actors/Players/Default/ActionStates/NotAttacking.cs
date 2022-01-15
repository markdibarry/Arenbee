using Arenbee.Framework;
using Arenbee.Framework.Enums;
using Arenbee.Framework.Actors;
using Godot;
using Arenbee.Framework.Constants;

namespace Arenbee.Assets.Players.ActionStates
{
    public class NotAttacking : State<Player>
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
                StateMachine.TransitionTo(new UnarmedAttack());
            }
        }
    }
}
using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Godot;

namespace Arenbee.Assets.Players.MoveStates
{
    public class Stagger : State<Player>
    {
        Timer _staggerTimer;
        bool _isStaggered = true;

        public override void Enter()
        {
            StateController.ActionStateMachine.TransitionTo(new None());
            StateController.PlayBase("Stagger");
            _staggerTimer = Actor.CreateOneShotTimer(0.4f);
            _staggerTimer.Timeout += OnStaggerTimeout;
        }

        public override void Update()
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
            if (Object.IsInstanceValid(_staggerTimer))
                _staggerTimer.QueueFree();
        }

        public override void CheckForTransitions()
        {
            if (!_isStaggered)
            {
                StateController.TransitionToInit();
            }
        }

        public void OnStaggerTimeout()
        {
            _isStaggered = false;
        }
    }
}

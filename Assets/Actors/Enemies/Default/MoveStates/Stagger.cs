using Arenbee.Framework;
using Arenbee.Framework.Actors;
using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Assets.Enemies.MoveStates
{
    public class Stagger : State<Enemy>
    {
        Timer _staggerTimer;
        bool _isStaggered = true;
        public override void Enter()
        {
            StateController.ActionStateMachine.TransitionTo(new None());
            _staggerTimer = Actor.CreateOneShotTimer(0.5f);
            _staggerTimer.Timeout += OnStaggerTimeout;
        }

        public override void Update(float delta)
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
                StateController.ResetMachines();
            }
        }

        public void OnStaggerTimeout()
        {
            _isStaggered = false;
        }
    }
}
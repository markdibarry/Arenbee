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
            AnimationName = "Stagger";
            StateController.ActionStateMachine.InputDisabled = true;
            StateController.BaseStateMachine.InputDisabled = true;
            StateController.PlayBase(AnimationName);
            _staggerTimer = Actor.CreateOneShotTimer(0.4f);
            _staggerTimer.Timeout += OnStaggerTimeout;
        }

        public override void Update(float delta)
        {
            CheckForTransitions();
        }

        public override void Exit()
        {
            StateController.ActionStateMachine.InputDisabled = false;
            StateController.BaseStateMachine.InputDisabled = false;
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

using Arenbee.Framework;
using Arenbee.Framework.Actors;

namespace Arenbee.Assets.Actors.Enemies.Default.State
{
    public class Stagger : HealthState
    {
        public Stagger()
        {
            AnimationName = "Stagger";
            BlockedStates = new ActorStateType[]
            {
                ActorStateType.Attack,
                ActorStateType.Jump,
                ActorStateType.Move
            };
        }

        float _staggerTimer;
        bool _isStaggered;

        public override void Enter(object[] args)
        {
            _staggerTimer = 1f;
            _isStaggered = true;
            Actor.PlaySoundFX("agh1.wav");
            PlayAnimation(AnimationName);
        }

        public override ActorState Update(float delta)
        {
            if (_staggerTimer > 0)
                _staggerTimer -= delta;
            else
                _isStaggered = false;
            return CheckForTransitions();
        }

        public override void Exit() { }

        public override ActorState CheckForTransitions()
        {
            if (Actor.Stats.IsKO())
                return GetState<Dead>();
            if (!_isStaggered)
                return GetState<Normal>();
            return null;
        }
    }
}

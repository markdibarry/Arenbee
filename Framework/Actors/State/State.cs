using Arenbee.Framework.Actors;
using Arenbee.Framework.Input;
using Godot;

namespace Arenbee.Framework
{
    public abstract class State<T> : IState where T : Actor
    {
        protected T Actor { get; private set; }
        public StateController StateController { get; set; }
        public IStateMachine StateMachine { get; set; }
        public ActorInputHandler InputHandler
        {
            get { return Actor.InputHandler; }
        }
        public bool IsInitialState { get; set; }
        public string AnimationName { get; set; }
        public bool SubscribedAnimation { get; set; }

        public virtual void Init()
        {
            Actor = (T)StateMachine.Actor;
            StateController = StateMachine.StateController;
        }

        public abstract void Enter();
        public abstract void Update(float delta);
        public abstract void Exit();
        public abstract void CheckForTransitions();

        protected void PlayAnimation(string animationName, bool force = false)
        {
            StateMachine.PlayAnimation(this, animationName, force);
        }

        protected void SubscribeAnimation()
        {
            Actor.AnimationPlayer.AnimationFinished += OnAnimationFinished;
            SubscribedAnimation = true;
        }

        protected void UnsubscribeAnimation()
        {
            if (SubscribedAnimation)
            {
                Actor.AnimationPlayer.AnimationFinished -= OnAnimationFinished;
                SubscribedAnimation = false;
            }
        }

        protected virtual void OnAnimationFinished(StringName animationName) { }
    }

    public class None : State<Actor>
    {
        public None() { IsInitialState = true; }
        public override void Enter()
        {
        }

        public override void Update(float delta)
        {
        }

        public override void Exit()
        {
        }

        public override void CheckForTransitions()
        {
        }
    }
}
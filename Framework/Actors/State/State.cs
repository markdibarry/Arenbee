using Arenbee.Framework.Actors;

namespace Arenbee.Framework
{
    public abstract class State<T> : IState where T : Actor
    {
        protected T Actor { get; private set; }
        public StateController StateController { get; set; }
        public IStateMachine StateMachine { get; set; }
        public bool IsInitialState { get; set; }

        public virtual void Init()
        {
            Actor = (T)StateMachine.Actor;
            StateController = StateMachine.StateController;
        }

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
        public abstract void CheckForTransitions();
    }

    public class None : State<Actor>
    {
        public None() { IsInitialState = true; }
        public override void Enter()
        {
        }

        public override void Update()
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
namespace Arenbee.Framework.Utility
{
    public abstract class State<TState, TStateMachine>
        where TState : State<TState, TStateMachine>
        where TStateMachine : StateMachine<TState, TStateMachine>
    {
        public TStateMachine StateMachine { get; private set; }

        public virtual void Init(TStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void Enter(object[] args = null) { Enter(); }
        public abstract TState Update(float delta);
        public virtual void Exit() { }
        public abstract TState CheckForTransitions();

        protected TState GetState<T>() where T : TState, new()
        {
            return StateMachine.GetState<T>();
        }
    }
}
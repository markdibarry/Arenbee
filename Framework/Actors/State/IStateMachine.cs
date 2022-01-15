namespace Arenbee.Framework.Actors
{
    public interface IStateMachine
    {
        Actor Actor { get; set; }
        StateController StateController { get; set; }
        IState InitialState { get; set; }
        IState State { get; set; }
        void Update();
        void TransitionTo(IState newState);
        void Init(IState initialState);
    }
}
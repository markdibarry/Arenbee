using GameCore.Actors;

namespace GameCore.Input
{
    public abstract class BehaviorTree
    {
        protected BehaviorTree(Actor actor)
        {
            _blackBoard = new BlackBoard();
            _root = SetupTree();
            _root.SetDependencies(actor, _blackBoard);
        }

        private readonly BTNode _root;
        private readonly BlackBoard _blackBoard;

        public void Update(float delta)
        {
            _root?.Evaluate(delta);
        }

        public void ClearBlackBoard()
        {
            _blackBoard.Clear();
        }

        protected abstract BTNode SetupTree();
    }
}
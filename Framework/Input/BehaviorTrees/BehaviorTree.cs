using Arenbee.Framework.Actors;

namespace Arenbee.Framework.Input
{
    public abstract class BehaviorTree
    {
        protected BehaviorTree(Actor actor)
        {
            _root = SetupTree();
            _root.SetDependencies(actor, _blackBoard);
        }

        private readonly BTNode _root = null;
        private readonly BlackBoard _blackBoard = new BlackBoard();

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
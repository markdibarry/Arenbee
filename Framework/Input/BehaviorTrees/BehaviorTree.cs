using Arenbee.Framework.Actors;

namespace Arenbee.Framework.Input
{
    public abstract class BehaviorTree
    {
        public BehaviorTree(Actor actor)
        {
            _root = SetupTree();
            _root.SetDependencies(actor, _blackBoard);
        }

        private readonly BTNode _root = null;
        private readonly BlackBoard _blackBoard = new BlackBoard();

        public void Update(float delta)
        {
            if (_root != null)
                _root.Evaluate(delta);
        }

        protected abstract BTNode SetupTree();
    }
}
using System.Collections.Generic;
using Arenbee.Framework.Actors;

namespace Arenbee.Framework.Input
{
    public class BTNode
    {
        public enum NodeState { Running, Failure, Success };

        public BTNode()
        {
            Parent = null;
            Children = new List<BTNode>();
        }

        public BTNode(List<BTNode> children)
            : this()
        {
            foreach (BTNode child in children)
                Attach(child);
        }

        public void SetDependencies(Actor actor, BlackBoard blackBoard)
        {
            Actor = actor;
            _blackBoard = blackBoard;
            Init();
            foreach (var child in Children)
                child.SetDependencies(actor, blackBoard);
        }

        private BlackBoard _blackBoard;
        public BTNode Parent { get; set; }
        protected List<BTNode> Children { get; }
        protected Actor Actor { get; private set; }
        protected NodeState State { get; set; }

        public virtual void Init() { }

        private void Attach(BTNode node)
        {
            node.Parent = this;
            Children.Add(node);
        }

        public virtual NodeState Evaluate(float delta)
        {
            return NodeState.Failure;
        }

        public void SetData(string key, object value)
        {
            _blackBoard[key] = value;
        }

        public object GetData(string key)
        {
            if (_blackBoard.TryGetValue(key, out object value))
                return value;
            return null;
        }

        public bool ClearData(string key)
        {
            return _blackBoard.Remove(key);
        }
    }
}
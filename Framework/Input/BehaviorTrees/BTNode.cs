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
        }

        public BTNode(List<BTNode> children)
        {
            foreach (BTNode child in children)
            {
                Attach(child);
            }
        }

        public void SetDependencies(Actor actor, BlackBoard blackBoard)
        {
            Actor = actor;
            BlackBoard = blackBoard;
            Init();
            foreach (var child in Children)
            {
                child.SetDependencies(actor, blackBoard);
            }
        }

        protected Actor Actor { get; private set; }
        protected BlackBoard BlackBoard { get; private set; }
        protected NodeState State { get; set; }
        public BTNode Parent { get; set; }
        protected List<BTNode> Children { get; set; } = new List<BTNode>();

        public virtual void Init()
        {
        }

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
            BlackBoard[key] = value;
        }

        public object GetData(string key)
        {
            if (BlackBoard.TryGetValue(key, out object value))
                return value;
            return null;
        }

        public bool ClearData(string key)
        {
            if (BlackBoard.ContainsKey(key))
            {
                BlackBoard.Remove(key);
                return true;
            }
            return false;
        }
    }
}
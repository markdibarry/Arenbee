using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GameCore.Actors;

namespace GameCore.Input;

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

    public void SetDependencies(AActorBody actor, BlackBoard blackBoard)
    {
        Actor = actor;
        _blackBoard = blackBoard;
        Init();
        foreach (var child in Children)
            child.SetDependencies(actor, blackBoard);
    }

    private BlackBoard _blackBoard = null!;
    public BTNode? Parent { get; set; }
    protected AActorBody Actor { get; private set; } = null!;
    protected List<BTNode> Children { get; }
    protected NodeState State { get; set; }

    public virtual void Init() { }

    private void Attach(BTNode node)
    {
        node.Parent = this;
        Children.Add(node);
    }

    public virtual NodeState Evaluate(double delta)
    {
        return NodeState.Failure;
    }

    public void SetData(string key, object value)
    {
        _blackBoard[key] = value;
    }

    public object? GetData(string key)
    {
        if (_blackBoard.TryGetValue(key, out object? value))
            return value;
        return null;
    }

    public bool TryGetData<T>(string key, [NotNullWhen(returnValue: true)] out T? value)
    {
        if (!_blackBoard.TryGetValue(key, out object? result) || result is not T)
        {
            value = default;
            return false;
        }
        value = (T)result;
        return true;
    }

    public bool ClearData(string key)
    {
        return _blackBoard.Remove(key);
    }
}

using System;
using Godot;
using Godot.Collections;

namespace GameCore.GodotNative;
#pragma warning disable IDE1006 // Naming Styles
public interface INode : IObject
{
    StringName Name { get; set; }
    Node Owner { get; set; }
    Node.ProcessModeEnum ProcessMode { get; set; }
    string SceneFilePath { get; set; }

    event Node.ChildEnteredTreeEventHandler ChildEnteredTree;
    event Node.ChildExitingTreeEventHandler ChildExitingTree;
    event Action Ready;
    event Action Renamed;
    event Action TreeEntered;
    event Action TreeExited;
    event Action TreeExiting;

    void AddChild(Node node, bool forceReadableName = false, Node.InternalMode @internal = Node.InternalMode.Disabled);
    void AddSibling(Node sibling, bool forceReadableName = false);
    Tween CreateTween();
    Node GetChild(int idx, bool includeInternal = false);
    T GetChild<T>(int idx, bool includeInternal = false) where T : class;
    int GetChildCount(bool includeInternal = false);
    T GetChildOrNull<T>(int idx, bool includeInternal = false) where T : class;
    Array<Node> GetChildren(bool includeInternal = false);
    Node GetNode(NodePath path);
    T GetNode<T>(NodePath path) where T : class;
    Node GetNodeOrNull(NodePath path);
    T GetNodeOrNull<T>(NodePath path) where T : class;
    Node GetParent();
    T GetParent<T>() where T : class;
    T GetParentOrNull<T>() where T : class;
    NodePath GetPath();
    NodePath GetPathTo(Node node, bool useUniquePath = false);
    SceneTree GetTree();
    bool HasNode(NodePath path);
    bool IsInGroup(StringName group);
    void MoveChild(Node childNode, int toIndex);
    void QueueFree();
    void RemoveChild(Node node);
    void RemoveFromGroup(StringName group);
    void Reparent(Node newParent, bool keepGlobalTransform = true);
    void _EnterTree();
    void _ExitTree();
    void _Input(InputEvent @event);
    void _PhysicsProcess(double delta);
    void _Process(double delta);
    void _Ready();
    void _UnhandledInput(InputEvent @event);
    void _UnhandledKeyInput(InputEvent @event);
}
#pragma warning restore IDE1006 // Naming Styles

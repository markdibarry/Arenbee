using GameCore.Extensions;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class EqualContainer : MarginContainer
{
    public static string GetScenePath() => GDEx.GetScenePath();
    public MarginContainer KeyContainer { get; set; }
    public MarginContainer ValueContainer { get; set; }

    public override void _Ready()
    {
        KeyContainer = GetNode<MarginContainer>("HBoxContainer/KeyContainer");
        ValueContainer = GetNode<MarginContainer>("HBoxContainer/ValueContainer");
        Resized += OnResize;
    }

    public virtual void OnResize()
    {
        ResizeItems(KeyContainer, ValueContainer);
    }

    protected void ResizeItems(Control itemA, Control itemB)
    {
        if (itemA.Size.X > itemB.Size.X)
            CustomMinimumSize = new Vector2I((int)itemA.Size.X * 2, 0);
        else if (itemB.Size.X > itemA.Size.X)
            CustomMinimumSize = new Vector2I((int)itemB.Size.X * 2, 0);
    }
}

using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Common;

[Tool]
public partial class PartyMemberOption : OptionItem
{
    public static string GetScenePath() => GDEx.GetScenePath();

    public Label NameLabel { get; set; } = null!;
    public PointContainer HPContainer { get; set; } = null!;
    public PointContainer MPContainer { get; set; } = null!;

    public override void _Ready()
    {
        NameLabel = GetNode<Label>("%Name");
        HPContainer = GetNode<PointContainer>("%HPContainer");
        MPContainer = GetNode<PointContainer>("%MPContainer");
    }
}

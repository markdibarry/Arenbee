using Arenbee.Assets.GUI.Menus.Common;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Godot;

[Tool]
public partial class PartyMemberOption : OptionItem
{
    public static string GetScenePath() => GDEx.GetScenePath();

    public Label NameLabel { get; set; }
    public PointContainer HPContainer { get; set; }
    public PointContainer MPContainer { get; set; }

    public override void _Ready()
    {
        NameLabel = GetNode<Label>("%Name");
        HPContainer = GetNode<PointContainer>("%HPContainer");
        MPContainer = GetNode<PointContainer>("%MPContainer");
    }
}

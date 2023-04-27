using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.PartyMenus.Equipment;

[Tool]
public partial class EquipSelectOption : KeyValueOption
{
    public static new string GetScenePath() => GDEx.GetScenePath();
}

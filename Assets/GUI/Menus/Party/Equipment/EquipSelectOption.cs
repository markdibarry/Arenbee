using Arenbee.Framework.Extensions;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Party.Equipment
{
    [Tool]
    public partial class EquipSelectOption : KeyValueOption
    {
        public static new string GetScenePath() => GDEx.GetScenePath();
    }
}
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class MainSubMenu : OptionSubMenu
    {
        public static new readonly string ScenePath = $"res://Assets/GUI/Menus/PartyMenu/{nameof(MainSubMenu)}.tscn";
        protected override void OnItemSelected(OptionItem optionItem)
        {
            base.OnItemSelected(optionItem);
            switch (optionItem.OptionValue)
            {
                case "Stats":
                    OpenStatsSubMenu();
                    break;
                case "Inventory":
                    OpenInventorySubMenu();
                    break;
                case "Equipment":
                    OpenEquipmentSubMenu();
                    break;
                case "Save":
                    OpenSaveGameConfirm();
                    break;
                case "Quit":
                    GameRoot.Instance.ResetToTitleScreen();
                    break;
                default:
                    break;
            }
        }

        private void OpenStatsSubMenu()
        {
            var statsScene = GD.Load<PackedScene>(StatsSubMenu.ScenePath);
            RaiseRequestedAddSubMenu(statsScene.Instantiate<SubMenu>());
        }

        private void OpenInventorySubMenu()
        {
            var inventoryScene = GD.Load<PackedScene>(InventorySubMenu.ScenePath);
            RaiseRequestedAddSubMenu(inventoryScene.Instantiate<SubMenu>());
        }

        private void OpenEquipmentSubMenu()
        {
            var equipmentScene = GD.Load<PackedScene>(EquipmentSubMenu.ScenePath);
            RaiseRequestedAddSubMenu(equipmentScene.Instantiate<SubMenu>());
        }

        private void OpenSaveGameConfirm()
        {
            var saveConfirmScene = GD.Load<PackedScene>(SaveConfirmSubMenu.ScenePath);
            RaiseRequestedAddSubMenu(saveConfirmScene.Instantiate<SubMenu>());
        }
    }
}

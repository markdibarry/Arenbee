using Arenbee.Framework.Constants;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class MainSubMenu : OptionSubMenu
    {
        public override void OnItemSelected(OptionItem optionItem)
        {
            base.OnItemSelected(optionItem);
            switch (optionItem.Value)
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

        public void OpenStatsSubMenu()
        {
            var statsScene = GD.Load<PackedScene>(PathConstants.StatsSubMenuPath);
            RaiseRequestedAddSubMenu(statsScene.Instantiate<SubMenu>());
        }

        public void OpenInventorySubMenu()
        {
            var inventoryScene = GD.Load<PackedScene>(PathConstants.InventorySubMenuPath);
            RaiseRequestedAddSubMenu(inventoryScene.Instantiate<SubMenu>());
        }

        public void OpenEquipmentSubMenu()
        {
            var equipmentScene = GD.Load<PackedScene>(PathConstants.EquipmentSubMenuPath);
            RaiseRequestedAddSubMenu(equipmentScene.Instantiate<SubMenu>());
        }

        public void OpenSaveGameConfirm()
        {
            var saveConfirmScene = GD.Load<PackedScene>(PathConstants.SaveConfirmSubMenuPath);
            RaiseRequestedAddSubMenu(saveConfirmScene.Instantiate<SubMenu>());
        }
    }
}

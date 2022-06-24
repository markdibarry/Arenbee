using Arenbee.Assets.GUI.Menus.Party.Equipment;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Party
{
    [Tool]
    public partial class MainSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();

        protected override void OnItemSelected()
        {
            base.OnItemSelected();
            var subMenuName = CurrentContainer.CurrentItem.GetData<string>("subMenu");
            if (subMenuName == null)
                return;

            switch (subMenuName)
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
                    QuitToTitle();
                    break;
            }
        }

        private void OpenStatsSubMenu()
        {
            RaiseRequestedAdd(GDEx.Instantiate<StatsSubMenu>(StatsSubMenu.GetScenePath()));
        }

        private void OpenInventorySubMenu()
        {
            RaiseRequestedAdd(GDEx.Instantiate<InventorySubMenu>(InventorySubMenu.GetScenePath()));
        }

        private void OpenEquipmentSubMenu()
        {
            RaiseRequestedAdd(GDEx.Instantiate<EquipmentSubMenu>(EquipmentSubMenu.GetScenePath()));
        }

        private void OpenSaveGameConfirm()
        {
            RaiseRequestedAdd(GDEx.Instantiate<SaveConfirmSubMenu>(SaveConfirmSubMenu.GetScenePath()));
        }

        private void QuitToTitle()
        {
            IsActive = false;
            GameRoot.Instance.QueueReset();
        }
    }
}

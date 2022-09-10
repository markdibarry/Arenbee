using System.Threading.Tasks;
using Arenbee.GUI.Menus.Party.Equipment;
using GameCore.Extensions;
using GameCore.GUI;
using GameCore.Utility;
using Godot;

namespace Arenbee.GUI.Menus.Party
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
            var tController = Locator.TransitionController;
            var request = new TransitionRequest(
                BasicLoadingScreen.GetScenePath(),
                TransitionType.Game,
                WipeTransition.GetScenePath(),
                FadeTransition.GetScenePath(),
                new string[] { Locator.Root?.TitleMenuScenePath },
                (loader) =>
                {
                    var titleMenuScene = loader.GetObject<PackedScene>(Locator.Root?.TitleMenuScenePath);
                    Locator.Root?.ResetToTitleScreenAsync(titleMenuScene);
                    return Task.CompletedTask;
                });
            tController.RequestTransition(request);
        }
    }
}

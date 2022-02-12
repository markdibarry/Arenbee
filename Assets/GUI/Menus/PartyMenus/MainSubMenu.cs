using System.Threading.Tasks;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Godot;

namespace Arenbee.Assets.GUI.Menus.PartyMenus
{
    [Tool]
    public partial class MainSubMenu : OptionSubMenu
    {
        public static string GetScenePath() => GDEx.GetScenePath();
        public AnimationPlayer AnimationPlayer { get; set; }
        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            AnimationPlayer = GetNode<AnimationPlayer>("TransitionFadeColor/AnimationPlayer");
        }

        protected override async Task TransitionIn()
        {
            Foreground.Modulate = Colors.Transparent;
            Background.Modulate = Colors.Transparent;
            Modulate = Colors.White;
            AnimationPlayer.Play("TransitionIn");
            await ToSignal(AnimationPlayer, "animation_finished");
            Foreground.Modulate = Colors.White;
            Background.Modulate = Colors.White;
            AnimationPlayer.Play("TransitionOut");
            await ToSignal(AnimationPlayer, "animation_finished");
        }

        protected override async Task TransitionOut()
        {
            AnimationPlayer.Play("TransitionIn");
            await ToSignal(AnimationPlayer, "animation_finished");
            Foreground.Modulate = Colors.Transparent;
            Background.Modulate = Colors.Transparent;
            AnimationPlayer.Play("TransitionOut");
            await ToSignal(AnimationPlayer, "animation_finished");
        }

        protected override void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
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
                    QuitToTitle();
                    break;
            }
        }

        private void OpenStatsSubMenu()
        {
            RaiseRequestedAddSubMenu(GDEx.Instantiate<StatsSubMenu>(StatsSubMenu.GetScenePath()));
        }

        private void OpenInventorySubMenu()
        {
            RaiseRequestedAddSubMenu(GDEx.Instantiate<InventorySubMenu>(InventorySubMenu.GetScenePath()));
        }

        private void OpenEquipmentSubMenu()
        {
            RaiseRequestedAddSubMenu(GDEx.Instantiate<EquipmentSubMenu>(EquipmentSubMenu.GetScenePath()));
        }

        private void OpenSaveGameConfirm()
        {
            RaiseRequestedAddSubMenu(GDEx.Instantiate<SaveConfirmSubMenu>(SaveConfirmSubMenu.GetScenePath()));
        }

        private void QuitToTitle()
        {
            IsActive = false;
            GameRoot.Instance.ResetToTitleScreen();
        }
    }
}

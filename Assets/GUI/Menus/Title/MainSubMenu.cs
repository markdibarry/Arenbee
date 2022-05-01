using System.Threading.Tasks;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game;
using Arenbee.Framework.GUI;
using Arenbee.Framework.Game.SaveData;
using Godot;

namespace Arenbee.Assets.GUI.Menus.Title
{
    [Tool]
    public partial class MainSubMenu : OptionSubMenu
    {
        public MainSubMenu()
        {
            PreventCancel = true;
            PreventCloseAll = true;
        }

        private OptionContainer _startOptions;
        public static string GetScenePath() => GDEx.GetScenePath();

        public override async Task TransitionOpenAsync()
        {
            var pos = _startOptions.Position;
            _startOptions.Position = new Vector2(pos.x, -_startOptions.Size.y);
            var tween = GetTree().CreateTween();
            tween.TweenProperty(_startOptions, "position:y", pos.y, 0.4f);
            await ToSignal(tween, "finished");
        }

        protected override void OnItemSelected(OptionContainer optionContainer, OptionItem optionItem)
        {
            base.OnItemSelected(optionContainer, optionItem);
            if (!optionItem.OptionData.TryGetValue("titleChoice", out string titleChoice))
                return;
            IsActive = false;
            switch (titleChoice)
            {
                case "Continue":
                    ContinueSavedGame();
                    break;
                case "NewGame":
                    StartNewGame();
                    break;
            }
        }

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            _startOptions = OptionContainers.Find(x => x.Name == "MainOptions");
        }

        private void StartNewGame()
        {
            RaiseRequestedClose(() => GameRoot.Instance.StartGame(SaveService.GetNewGame()));
        }

        private void ContinueSavedGame()
        {
            RaiseRequestedClose(() => GameRoot.Instance.StartGame(SaveService.LoadGame()));
        }
    }
}

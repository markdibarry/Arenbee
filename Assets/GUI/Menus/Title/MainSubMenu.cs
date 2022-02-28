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
            : base()
        {
            PreventCancel = true;
            PreventCloseAll = true;
        }

        public static string GetScenePath() => GDEx.GetScenePath();
        public AnimationPlayer AnimationPlayer { get; set; }
        private OptionContainer _optionContainer;

        protected override void SetNodeReferences()
        {
            base.SetNodeReferences();
            AnimationPlayer = GetNode<AnimationPlayer>("TransitionFadeColor/AnimationPlayer");
            _optionContainer = Foreground.GetNode<OptionContainer>("MainOptions");
            OptionContainers.Add(_optionContainer);
        }

        protected override async Task TransitionInAsync()
        {
            Foreground.Modulate = Colors.Transparent;
            Background.Modulate = Colors.Transparent;
            Modulate = Colors.White;
            AnimationPlayer.Play("TransitionIn");
            await ToSignal(AnimationPlayer, "animation_finished");
            GameRoot.Instance?.EndCurrentgame();
            Foreground.Modulate = Colors.White;
            Background.Modulate = Colors.White;
            AnimationPlayer.Play("TransitionOut");
            await ToSignal(AnimationPlayer, "animation_finished");
        }

        protected override async Task TransitionOutAsync()
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

        private void StartNewGame()
        {
            StartGame(SaveService.GetNewGame());
        }

        private void ContinueSavedGame()
        {
            StartGame(SaveService.LoadGame());
        }

        private async void StartGame(GameSave gameSave = null)
        {
            var gameRoot = GameRoot.Instance;
            gameRoot.CurrentGame = GDEx.Instantiate<GameSession>(GameSession.GetScenePath());
            gameRoot.CurrentGameContainer.AddChild((Node)gameRoot.CurrentGame);
            gameRoot.CurrentGame.Init(gameSave);
            await CloseSubMenuAsync();
            gameRoot.CurrentGame.ProcessMode = ProcessModeEnum.Inherit;
        }
    }
}
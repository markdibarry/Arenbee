using System;
using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Arenbee.Assets.GUI.Menus.Party;
using Arenbee.Framework.GUI.Dialog;
using Godot;
using Arenbee.Framework.Game.SaveData;
using Arenbee.Framework.Input;
using Arenbee.Framework.Utility;
using Arenbee.Assets.GUI;

namespace Arenbee.Framework.Game
{
    public partial class GameSession : GameSessionBase
    {
        public GameSession()
        {
            Locator.ProvidePlayerParty(null);
            Party = Locator.GetParty();
            SessionState = new SessionState();
            _menuInput = Locator.GetMenuInput();
        }

        public static string GetScenePath() => GDEx.GetScenePath();
        private readonly GUIInputHandler _menuInput;
        private readonly PackedScene _partyMenuScene = GD.Load<PackedScene>(MainSubMenu.GetScenePath());
        private Node2D _currentAreaContainer;
        private HUD _hud;
        private Menu _partyMenu;

        public override void _ExitTree()
        {
            Party.Free();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_menuInput.Start.IsActionJustPressed)
                OpenPartyMenu();
        }

        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.Disabled;
            SetNodeReferences();
            SubscribeEvents();
        }

        public override void AddAreaScene(AreaScene areaScene)
        {
            if (IsInstanceValid(CurrentAreaScene))
            {
                GD.PrintErr("AreaScene already active. Cannot add new AreaScene.");
                return;
            }
            CurrentAreaScene = areaScene;
            CurrentAreaScene.ProcessMode = ProcessModeEnum.Disabled;
            _currentAreaContainer.AddChild(areaScene);
            _hud.SubscribeAreaSceneEvents(areaScene);
        }

        public override void Init(GameSave gameSave)
        {
            Locator.ProvidePlayerParty(new PlayerParty(gameSave.ActorData, gameSave.Items));
            Party = Locator.GetParty();
            SessionState = gameSave.SessionState;
            InitAreaScene();
            CurrentAreaScene.AddPlayer();
            CurrentAreaScene.ProcessMode = ProcessModeEnum.Inherit;
        }

        public override void OpenDialog(string path)
        {
            if (_partyMenu.GetChildCount() == 0)
                DialogController.StartDialog(path);
        }

        public override void RemoveAreaScene()
        {
            CurrentAreaScene?.RemovePlayer();
            CurrentAreaScene?.QueueFree();
            //CurrentAreaScene = null;
        }

        public override async void ReplaceScene(AreaScene areaScene)
        {
            CurrentAreaScene.ProcessMode = ProcessModeEnum.Disabled;
            Transition.AnimationPlayer.Play("TransitionIn");
            await ToSignal(Transition.AnimationPlayer, "animation_finished");
            CallDeferred("RemoveAreaScene");
            while (IsInstanceValid(CurrentAreaScene))
            {
                await ToSignal(GetTree(), "physics_frame");
            }
            AddAreaScene(areaScene);
            areaScene.AddPlayer();
            Transition.AnimationPlayer.Play("TransitionOut");
            await ToSignal(Transition.AnimationPlayer, "animation_finished");
            CurrentAreaScene.ProcessMode = ProcessModeEnum.Inherit;
        }

        private void InitAreaScene()
        {
            // TODO: Make game
            if (CurrentAreaScene == null)
            {
                var demoAreaScene = GDEx.Instantiate<AreaScene>(PathConstants.DemoLevel1);
                AddAreaScene(demoAreaScene);
            }
        }

        private void OnDialogStarted()
        {
            Party.DisableUserInput(true);
        }

        private void OnDialogEnded()
        {
            Party.DisableUserInput(false);
        }

        private void OnPartyMenuRootClosed(object sender, EventArgs e)
        {
            CurrentAreaScene.ProcessMode = ProcessModeEnum.Inherit;
            _hud.ProcessMode = ProcessModeEnum.Inherit;
        }

        private void OpenPartyMenu()
        {
            if (_partyMenu.GetChildCount() == 0)
            {
                CurrentAreaScene.ProcessMode = ProcessModeEnum.Disabled;
                _hud.ProcessMode = ProcessModeEnum.Disabled;
                var partySubMenu = _partyMenuScene.Instantiate<SubMenu>();
                _partyMenu.AddSubMenu(partySubMenu);
            }
        }

        private void SetNodeReferences()
        {
            _hud = GetNode<HUD>("HUD");
            _currentAreaContainer = GetNode<Node2D>("CurrentAreaSceneContainer");
            _partyMenu = GetNode<Menu>("Menu");
            Transition = GetNode<TransitionFadeColor>("Transition/TransitionFadeColor");
            DialogController = GetNode<DialogController>("DialogController");
        }

        private void SubscribeEvents()
        {
            DialogController.DialogStarted += OnDialogStarted;
            DialogController.DialogEnded += OnDialogEnded;
            _partyMenu.RootSubMenuClosed += OnPartyMenuRootClosed;
        }

        private void UnsubscribeEvents()
        {
            _partyMenu.RootSubMenuClosed -= OnPartyMenuRootClosed;
            DialogController.DialogStarted -= OnDialogStarted;
            DialogController.DialogEnded -= OnDialogEnded;
        }
    }
}

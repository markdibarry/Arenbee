using System;
using Arenbee.Framework.Actors;
using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Arenbee.Framework.SaveData;
using Godot;

namespace Arenbee.Framework.Game
{
    public partial class GameSession : Node2D
    {
        public GameSession()
        {
            Party = new Party();
            SessionState = new SessionState();
        }

        public static string GetScenePath() => GDEx.GetScenePath();
        private readonly PackedScene _partyMenuScene = GD.Load<PackedScene>(PathConstants.PartyMenuPath);
        private Menu _partyMenu;
        private Node2D _currentAreaContainer;
        private HUD _hud;
        public AreaScene CurrentAreaScene { get; private set; }
        public Party Party { get; private set; }
        public SessionState SessionState { get; private set; }
        public TransitionFadeColor Transition { get; private set; }

        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.Disabled;
            SetNodeReferences();
        }

        public void SetNodeReferences()
        {
            _hud = GetNode<HUD>("HUD");
            _currentAreaContainer = GetNode<Node2D>("CurrentAreaSceneContainer");
            Transition = GetNode<TransitionFadeColor>("Transition/TransitionFadeColor");
        }

        public void Init(GameSave gameSave)
        {
            Party = new Party(gameSave.ActorInfos, gameSave.Items);
            SessionState = gameSave.SessionState;
            InitAreaScene();
            CurrentAreaScene.AddPlayer();
            CurrentAreaScene.ProcessMode = ProcessModeEnum.Inherit;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (GameRoot.MenuInput.Start.IsActionJustPressed)
            {
                OpenPartyMenu();
            }
        }

        public override void _ExitTree()
        {
            Party.Free();
        }

        public void AddAreaScene(AreaScene areaScene)
        {
            if (!IsInstanceValid(CurrentAreaScene))
            {
                CurrentAreaScene = areaScene;
                CurrentAreaScene.ProcessMode = ProcessModeEnum.Disabled;
                _currentAreaContainer.AddChild(areaScene);
                _hud.SubscribeAreaSceneEvents(areaScene);
            }
            else
            {
                GD.PrintErr("AreaScene already active. Cannot add new AreaScene.");
            }
        }

        public void RemoveAreaScene()
        {
            CurrentAreaScene?.RemovePlayer();
            CurrentAreaScene?.QueueFree();
            //CurrentAreaScene = null;
        }

        public async void ReplaceScene(AreaScene areaScene)
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

        private void OpenPartyMenu()
        {
            if (!IsInstanceValid(_partyMenu))
            {
                CurrentAreaScene.ProcessMode = ProcessModeEnum.Disabled;
                _partyMenu = _partyMenuScene.Instantiate<Menu>();
                _partyMenu.RootSubMenuClosed += OnPartyMenuRootClosed;
                AddChild(_partyMenu);
            }
        }

        private void OnPartyMenuRootClosed(object sender, EventArgs e)
        {
            _partyMenu.RootSubMenuClosed -= OnPartyMenuRootClosed;
            CurrentAreaScene.ProcessMode = ProcessModeEnum.Inherit;
        }
    }
}

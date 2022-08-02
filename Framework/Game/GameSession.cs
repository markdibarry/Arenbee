using Arenbee.Assets.GUI;
using Arenbee.Assets.GUI.Menus;
using Arenbee.Framework.Actors;
using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game.SaveData;
using Arenbee.Framework.GUI;
using Arenbee.Framework.GUI.Dialogs;
using Arenbee.Framework.Input;
using Arenbee.Framework.Statistics;
using Godot;
using static Arenbee.Framework.Actors.Actor;

namespace Arenbee.Framework.Game
{
    public partial class GameSession : Node2D
    {
        public GameSession()
        {
            Party = new PlayerParty();
            SessionState = new SessionState();
            _partyMenuScene = GD.Load<PackedScene>(PartyMenu.GetScenePath());
        }

        public static string GetScenePath() => GDEx.GetScenePath();
        private Node2D _areaSceneContainer;
        private HUD _hud;
        private GUIController _guiController;
        private readonly PackedScene _partyMenuScene;
        public AreaScene CurrentAreaScene { get; private set; }
        public PlayerParty Party { get; private set; }
        public SessionState SessionState { get; private set; }
        public CanvasLayer Transition { get; private set; }
        public delegate void AreaSceneHandler(AreaScene newScene);
        public delegate void PauseChangedHandler(ProcessModeEnum processMode);
        public event ActorHandler ActorAddedToArea;
        public event ActorHandler ActorRemovedFromArea;
        public event AreaSceneHandler AreaSceneAdded;
        public event AreaSceneHandler AreaSceneRemoved;

        public override void _ExitTree()
        {
            //Party.Free();
        }

        public void HandleInput(GUIInputHandler menuInput, float delta)
        {
            if (menuInput.Start.IsActionJustPressed && !_guiController.GUIActive)
                OpenPartyMenuAsync();
        }

        public override void _Process(float delta)
        {

        }

        public override void _Ready()
        {
            SetNodeReferences();
        }

        public void AddAreaScene(AreaScene areaScene)
        {
            if (IsInstanceValid(CurrentAreaScene))
            {
                GD.PrintErr("AreaScene already active. Cannot add new AreaScene.");
                return;
            }
            CurrentAreaScene = areaScene;
            _areaSceneContainer.AddChild(areaScene);
            SubscribeAreaEvents(areaScene);
            AreaSceneAdded?.Invoke(areaScene);
            areaScene.AddPlayer(0);
        }

        public void Init(GameSave gameSave)
        {
            _guiController = GameRoot.Instance.GUIController;
            Party = new PlayerParty(gameSave.ActorData, gameSave.Items);
            SessionState = gameSave.SessionState;
            InitAreaScene();
        }

        public void OnGameStateChanged(GameState gameState)
        {
            if (gameState.MenuActive)
                Pause();
            else
                Resume();
            CurrentAreaScene?.OnGameStateChanged(gameState);
        }

        public void OpenDialog(string path)
        {
            _guiController.OpenDialog(path);
        }

        public void Pause()
        {
            CurrentAreaScene.ProcessMode = ProcessModeEnum.Disabled;
            _hud.ProcessMode = ProcessModeEnum.Disabled;
        }

        public void Resume()
        {
            CurrentAreaScene.ProcessMode = ProcessModeEnum.Inherit;
            _hud.ProcessMode = ProcessModeEnum.Inherit;
        }

        public void RemoveAreaScene()
        {
            if (CurrentAreaScene == null)
                return;
            CurrentAreaScene.RemovePlayer();
            _areaSceneContainer.RemoveChild(CurrentAreaScene);
            UnsubscribeAreaEvents(CurrentAreaScene);
            AreaSceneRemoved?.Invoke(CurrentAreaScene);
            CurrentAreaScene.QueueFree();
            CurrentAreaScene = null;
        }

        public void ReplaceScene(AreaScene areaScene)
        {
            //TODO TransitionStart
            RemoveAreaScene();
            AddAreaScene(areaScene);

            //TODO TransitionEnd
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

        private void OnActorAdded(Actor actor)
        {
            _hud.OnActorAdded(actor);
            ActorAddedToArea?.Invoke(actor);
        }

        private void OnActorDamaged(Actor actor, DamageData damageData)
        {
            _hud.OnActorDamaged(actor, damageData);
            SessionState.OnActorDamaged(actor, damageData);
        }

        private void OnActorDefeated(Actor actor)
        {
            _hud.OnActorDefeated(actor);
            SessionState.OnActorDefeated(actor);
        }

        private void OnActorRemoved(Actor actor)
        {
            ActorRemovedFromArea?.Invoke(actor);
        }

        private void OnPlayerModChanged(Actor actor, ModChangeData modChangeData)
        {
            _hud.OnPlayerModChanged(actor, modChangeData);
        }

        private void OnPlayerStatsChanged(Actor actor)
        {
            _hud.OnPlayerStatsChanged(actor);
        }

        private async void OpenPartyMenuAsync()
        {
            await _guiController.OpenMenuAsync(_partyMenuScene);
        }

        private void SetNodeReferences()
        {
            _hud = GetNode<HUD>("HUD");
            _areaSceneContainer = GetNode<Node2D>("AreaSceneContainer");
            Transition = GetNode<CanvasLayer>("Transition");
        }

        private void SubscribeAreaEvents(AreaScene areaScene)
        {
            areaScene.ActorAdded += OnActorAdded;
            areaScene.ActorRemoved += OnActorRemoved;
            areaScene.ActorDamaged += OnActorDamaged;
            areaScene.ActorDefeated += OnActorDefeated;
            areaScene.PlayerModChanged += OnPlayerModChanged;
            areaScene.PlayerStatsChanged += OnPlayerStatsChanged;
        }
        private void UnsubscribeAreaEvents(AreaScene areaScene)
        {
            areaScene.ActorAdded -= OnActorAdded;
            areaScene.ActorRemoved -= OnActorRemoved;
            areaScene.ActorDamaged -= OnActorDamaged;
            areaScene.ActorDefeated -= OnActorDefeated;
            areaScene.PlayerModChanged -= OnPlayerModChanged;
            areaScene.PlayerStatsChanged -= OnPlayerStatsChanged;
        }
    }
}

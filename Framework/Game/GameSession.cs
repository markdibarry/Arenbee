using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.Game.SaveData;
using Arenbee.Framework.Input;
using Arenbee.Framework.Utility;
using Arenbee.Assets.GUI;
using Godot;

namespace Arenbee.Framework.Game
{
    public partial class GameSession : Node2D
    {
        public GameSession()
        {
            Party = new PlayerParty();
            SessionState = new SessionState();
            _menuInput = Locator.GetMenuInput();
        }

        public static string GetScenePath() => GDEx.GetScenePath();
        private readonly GUIInputHandler _menuInput;
        private Node2D _areaSceneContainer;
        private HUD _hud;
        public AreaScene CurrentAreaScene { get; private set; }
        public PlayerParty Party { get; private set; }
        public SessionState SessionState { get; private set; }
        public CanvasLayer Transition { get; private set; }

        public override void _ExitTree()
        {
            //Party.Free();
        }

        public override void _Process(float delta)
        {
            if (_menuInput.Start.IsActionJustPressed)
                OpenPartyMenu();
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
            areaScene.AddPlayer(0);
            _hud.SubscribeAreaSceneEvents(areaScene);
        }

        public void Init(GameSave gameSave)
        {
            Party = new PlayerParty(gameSave.ActorData, gameSave.Items);
            SessionState = gameSave.SessionState;
            InitAreaScene();
        }

        public void OpenDialog(string path)
        {
            GameRoot.Instance.DialogController.StartDialog(path);
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

        private void OpenPartyMenu()
        {
            GameRoot.Instance.MenuController.OpenPartyMenu();
        }

        private void SetNodeReferences()
        {
            _hud = GetNode<HUD>("HUD");
            _areaSceneContainer = GetNode<Node2D>("AreaSceneContainer");
            Transition = GetNode<CanvasLayer>("Transition");
        }
    }
}

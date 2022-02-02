using System;
using System.Linq;
using Arenbee.Assets.Actors.Players.AdyNS;
using Arenbee.Assets.Input;
using Arenbee.Framework.Actors;
using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Constants;
using Arenbee.Framework.Extensions;
using Arenbee.Framework.GUI;
using Arenbee.Framework.SaveData;
using Godot;

namespace Arenbee.Framework.Game
{
    public partial class GameSession : Node
    {
        public GameSession()
        {
            Party = new Party();
            SessionState = new SessionState();
        }

        public GameSession(GameSave gameSave)
        {
            Party = new Party(gameSave.ActorInfos, gameSave.Items);
            SessionState = gameSave.SessionState;
        }

        public AreaScene CurrentAreaScene { get; private set; }
        public Party Party { get; private set; }
        public SessionState SessionState { get; private set; }
        private Menu _partyMenu;
        private readonly PackedScene _partyMenuScene = GD.Load<PackedScene>(PathConstants.PartyMenuPath);

        public override void _Ready()
        {
            SetNodeReferences();
            Init();
        }

        private void SetNodeReferences()
        {
            CurrentAreaScene = this.GetChildren<AreaScene>().FirstOrDefault();
        }

        private void Init()
        {
            if (CurrentAreaScene == null)
            {
                PackedScene demoLevelScene = ResourceLoader.Load<PackedScene>(PathConstants.DemoLevel);
                CurrentAreaScene = demoLevelScene.Instantiate<AreaScene>();
                AddChild(CurrentAreaScene);
            }
            InitParty();
            InitHUD();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (GameRoot.MenuInput.Start.IsActionJustPressed)
            {
                OpenPartyMenu();
            }
        }

        private void InitHUD()
        {
            if (CurrentAreaScene.HUD != null)
            {
                var players = CurrentAreaScene.PlayersContainer
                    .GetChildren<Actor>();
                var enemies = CurrentAreaScene.EnemiesContainer
                    .GetChildren<Actor>();
                CurrentAreaScene.HUD.SubscribeEvents(players);
                CurrentAreaScene.HUD.SubscribeEvents(enemies);
            }
        }

        private void InitParty()
        {
            Actor player1 = Party.Actors.FirstOrDefault();
            if (player1 == null)
            {
                var actorScene = GD.Load<PackedScene>(Ady.ScenePath);
                player1 = actorScene.Instantiate<Actor>();
                player1.Inventory = Party.Inventory;
                Party.Actors.Add(player1);
            }

            player1.GlobalPosition = CurrentAreaScene.PlayerSpawnPoint.GlobalPosition;
            player1.AddChild(new Player1InputHandler());
            player1.AddChild(new Camera2D() { LimitLeft = 0, LimitBottom = 270, Current = true });
            CurrentAreaScene.PlayersContainer.AddChild(player1);
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
using System;
using System.Linq;
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
        public Party Party { get; set; }
        public AreaScene CurrentAreaScene { get; set; }
        public SessionState SessionState { get; set; }
        private Menu _partyMenu;
        private readonly PackedScene _partyMenuScene = GD.Load<PackedScene>(PathConstants.PartyMenuPath);

        public override void _Ready()
        {
            SetDefaultValues();
            SetNodeReferences();
            Init();
        }

        private void SetDefaultValues()
        {
            if (Party == null) Party = new Party();
            if (SessionState == null) SessionState = new SessionState();
        }

        private void SetNodeReferences()
        {
            CurrentAreaScene = this.GetChildOrNullButActually<AreaScene>(0);
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
            if (Godot.Input.IsActionJustPressed("intBool"))
            {
                SaveService.SaveGame(this);
            }

            if (GameRoot.MenuInput.Start.IsActionJustPressed)
            {
                OpenPartyMenu();
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

        public void ApplySaveData(GameSave gameSave)
        {
            if (gameSave == null) return;
            Party = new Party();
            SessionState = gameSave.SessionState;
            foreach (var actorInfo in gameSave.ActorInfos)
            {
                var actorScene = GD.Load<PackedScene>(actorInfo.ActorPath);
                var actor = actorScene.Instantiate<Actor>();
                actor.Inventory = gameSave.Inventory;
                actor.Equipment = actorInfo.Equipment;
                actor.Stats = actorInfo.Stats;
                Party.Actors.Add(actor);
            }
        }

        private void InitParty()
        {
            Actor player1 = Party.Actors.FirstOrDefault();
            if (player1 == null)
            {
                var actorScene = GD.Load<PackedScene>(PathConstants.AdyPath);
                player1 = actorScene.Instantiate<Actor>();
                player1.Inventory = Party.Inventory;
                //player1.Equipment = new Equipment();
                //player1.Equipment.GetSlot(EquipmentSlotName.Weapon).SetItem(ItemDB.GetItem("HockeyStick"));
                Party.Actors.Add(player1);
            }

            player1.GlobalPosition = CurrentAreaScene.PlayerSpawnPoint.GlobalPosition;
            player1.AddChild(new Player1InputHandler());
            player1.AddChild(new Camera2D() { LimitLeft = 0, LimitBottom = 270, Current = true });
            CurrentAreaScene.PlayersContainer.AddChild(player1);
        }

        private void InitHUD()
        {
            if (CurrentAreaScene.HUD != null)
            {
                var players = CurrentAreaScene.PlayersContainer
                    .GetChildren().OfType<Actor>();
                var enemies = CurrentAreaScene.GetNodeOrNull<Node>("Enemies")
                    .GetChildren().OfType<Actor>();
                CurrentAreaScene.HUD.SubscribeEvents(players);
                CurrentAreaScene.HUD.SubscribeEvents(enemies);
            }
        }
    }
}
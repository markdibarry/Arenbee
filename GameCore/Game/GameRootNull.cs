using GameCore.Game.SaveData;

namespace GameCore.Game
{
    public partial class GameRootNull : GameRootBase
    {
        public override void _Ready() { }

        protected override void SetNodeReferences() { }

        protected override void Init() { }

        protected override void ProvideLocatorReferences() { }

        public override void _Process(float delta) { }

        public override async void ResetToTitleScreenAsync() { }

        public override void EndCurrentgame() { }

        public override void QueueReset() { }

        public override void StartGame(GameSave gameSave = null) { }
    }
}

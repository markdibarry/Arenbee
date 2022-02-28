using Arenbee.Framework.AreaScenes;
using Arenbee.Framework.Game.SaveData;

namespace Arenbee.Framework.Game
{
    public partial class GameSessionNull : GameSessionBase
    {
        public override void AddAreaScene(AreaScene areaScene) { }
        public override void Init(GameSave gameSave) { }
        public override void OpenDialog(string path) { }
        public override void RemoveAreaScene() { }
        public override void ReplaceScene(AreaScene areaScene) { }
    }
}

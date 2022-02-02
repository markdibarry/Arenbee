using Arenbee.Framework.Game;
using Godot;
using Newtonsoft.Json;

namespace Arenbee.Framework.SaveData
{
    public static class SaveService
    {
        private static readonly string s_savePath = "user://gamesave.json";
        public static void SaveGame(GameSession gameSession)
        {
            var gameSave = new GameSave(gameSession);
            string saveString = JsonConvert.SerializeObject(gameSave, Formatting.Indented);
            var file = new File();
            file.Open(s_savePath, File.ModeFlags.Write);
            file.StoreString(saveString);
            file.Close();
        }

        public static GameSave LoadGame()
        {
            var file = new File();
            if (!file.FileExists(s_savePath)) return null;
            file.Open(s_savePath, File.ModeFlags.Read);
            string content = file.GetAsText();
            file.Close();
            GameSave gameSave = JsonConvert.DeserializeObject<GameSave>(content);
            return gameSave;
        }
    }
}
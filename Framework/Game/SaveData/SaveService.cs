﻿using Arenbee.Framework.Utility.JsonConverters;
using Godot;
using System.Text.Json;

namespace Arenbee.Framework.Game.SaveData
{
    public static class SaveService
    {
        private const string SavePath = "user://gamesave.json";
        private const string NewGamePath = "user://newgame.json";

        public static GameSave GetNewGame()
        {
            return LoadSavedGame(NewGamePath);
        }

        public static GameSave LoadGame()
        {
            return LoadSavedGame(SavePath);
        }

        private static GameSave LoadSavedGame(string path)
        {
            var file = new File();
            if (!File.FileExists(path)) return null;
            file.Open(path, File.ModeFlags.Read);
            string content = file.GetAsText();
            file.Close();
            var options = new JsonSerializerOptions();
            options.Converters.Add(new StatsNotifierConverter());
            return JsonSerializer.Deserialize<GameSave>(content, options);
        }

        public static void SaveGame(GameSession gameSession)
        {
            var gameSave = new GameSave(gameSession);
            var options = new JsonSerializerOptions();
            options.Converters.Add(new StatsNotifierConverter());
            options.WriteIndented = true;
            string saveString = JsonSerializer.Serialize(gameSave, options);
            var file = new File();
            file.Open(SavePath, File.ModeFlags.Write);
            file.StoreString(saveString);
            file.Close();
        }
    }
}

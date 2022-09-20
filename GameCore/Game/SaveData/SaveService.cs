using GameCore.Utility.JsonConverters;
using Godot;
using System.Collections.Generic;
using System.Text.Json;

namespace GameCore.SaveData;

public static class SaveService
{
    public static GameSave LoadGame(string path)
    {
        return LoadSavedGame(path);
    }

    public static List<GameSave> GetGameSaves()
    {
        List<GameSave> gamesaves = new();
        for (int i = 1; i <= 3; i++)
        {
            string savepath = $"{Config.SavePath}{Config.SavePrefix}{i}.json";
            var gamesave = LoadSavedGame(savepath);
            if (gamesave != null)
                gamesaves.Add(gamesave);
        }
        return gamesaves;
    }

    private static GameSave LoadSavedGame(string path)
    {
        var file = new File();
        if (!File.FileExists(path))
            return null;
        file.Open(path, File.ModeFlags.Read);
        string content = file.GetAsText();
        file.Close();
        var options = new JsonSerializerOptions();
        options.Converters.Add(new StatsNotifierConverter());
        return JsonSerializer.Deserialize<GameSave>(content, options);
    }

    public static void SaveGame(int saveId, GameSessionBase gameSession)
    {
        var gameSave = new GameSave(saveId, gameSession);
        var options = new JsonSerializerOptions();
        options.Converters.Add(new StatsNotifierConverter());
        options.WriteIndented = true;
        string saveString = JsonSerializer.Serialize(gameSave, options);
        var file = new File();
        string savepath = $"{Config.SavePath}{Config.SavePrefix}{saveId}.json";
        file.Open(savepath, File.ModeFlags.Write);
        file.StoreString(saveString);
        file.Close();
    }
}

using GameCore.Utility.JsonConverters;
using Godot;
using System.Text.Json;

namespace GameCore.SaveData;

public static class SaveService
{
    public static GameSave GetNewGame()
    {
        return LoadSavedGame(Config.NewGamePath);
    }

    public static GameSave LoadGame(string path)
    {
        return LoadSavedGame(path);
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

    public static void SaveGame(GameSessionBase gameSession)
    {
        var gameSave = new GameSave(gameSession);
        var options = new JsonSerializerOptions();
        options.Converters.Add(new StatsNotifierConverter());
        options.WriteIndented = true;
        string saveString = JsonSerializer.Serialize(gameSave, options);
        var file = new File();
        file.Open(Config.SavePath, File.ModeFlags.Write);
        file.StoreString(saveString);
        file.Close();
    }
}

using System.Text.Json;
using Godot;

namespace GameCore.GUI.Dialogs;

public static class DialogLoader
{
    public static DialogPart[] Load(string path)
    {
        string fullPath = $"{Config.DialogPath}{path}.json";
        var file = new File();
        if (!File.FileExists(fullPath))
            return null;
        file.Open(fullPath, File.ModeFlags.Read);
        string content = file.GetAsText();
        file.Close();
        return JsonSerializer.Deserialize<DialogPart[]>(content);
    }
}

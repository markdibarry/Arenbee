using Arenbee.Framework.Constants;
using Godot;
using Newtonsoft.Json;

namespace Arenbee.Framework.GUI.Dialog
{
    public static class DialogLoader
    {
        public static DialogPart[] Load(string path)
        {
            string fullPath = $"{PathConstants.DialogPath}{path}.json";
            var file = new File();
            if (!file.FileExists(fullPath)) return null;
            file.Open(fullPath, File.ModeFlags.Read);
            string content = file.GetAsText();
            file.Close();
            return JsonConvert.DeserializeObject<DialogPart[]>(content);
        }
    }
}
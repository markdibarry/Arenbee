using System;
using Godot;

namespace GameCore.GUI;

public class GUIOpenRequest
{
    public GUIOpenRequest(string path)
    {
        Path = path;
    }

    public GUIOpenRequest(PackedScene packedScene)
    {
        PackedScene = packedScene;
    }

    public Action Callback { get; set; }
    public bool IsDialog { get; set; }
    public string Path { get; set; }
    public PackedScene PackedScene { get; set; }
    public bool PreventAnimation { get; set; }
    public object Data { get; set; }
}

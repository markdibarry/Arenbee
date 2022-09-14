using System;
using System.Collections.Generic;
using Godot;

namespace GameCore.GUI;

public class MenuOpenRequest
{
    public MenuOpenRequest(string path)
    {
        Path = path;
    }

    public MenuOpenRequest(PackedScene packedScene)
    {
        PackedScene = packedScene;
    }

    public Action Callback { get; set; }
    public string Path { get; set; }
    public PackedScene PackedScene { get; set; }
    public bool PreventAnimation { get; set; }
    public Dictionary<string, object> GrabBag { get; set; } = new();
}

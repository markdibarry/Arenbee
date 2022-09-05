using System;

namespace GameCore.GUI.Menus;

public class GUILayerCloseRequest
{
    public GUILayer Layer { get; set; }
    public Action Callback { get; set; }
    public bool CloseAll { get; set; }
}

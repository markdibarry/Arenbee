using System;

namespace GameCore.GUI;

public class GUILayerCloseRequest
{
    public Action Callback { get; set; }
    public CloseRequestType CloseRequestType { get; set; }
    public GUILayer Layer { get; set; }
    public bool PreventAnimation { get; set; }
}

public enum CloseRequestType
{
    ProvidedLayer,
    AllLayers,
    AllMenus,
    AllDialog
}

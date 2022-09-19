using System;

namespace GameCore.GUI;

public class GUICloseRequest
{
    public Action Callback { get; set; }
    public Type CascadeTo { get; set; }
    public CloseRequestType CloseRequestType { get; set; }
    public bool PreventAnimation { get; set; }
    public object Data { get; set; }
}

public enum CloseRequestType
{
    SubLayer,
    Layer,
    AllLayers
}

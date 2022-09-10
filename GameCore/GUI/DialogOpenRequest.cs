using System;

namespace GameCore.GUI;

public class DialogOpenRequest
{
    public DialogOpenRequest(string path)
    {
        Path = path;
    }

    public Action Callback { get; set; }
    public string Path { get; set; }
    public bool PreventAnimation { get; set; }
}
﻿namespace GameCore.GUI.GameDialog;

public class Section : IStatement
{
    public string Name { get; set; } = string.Empty;
    public GoTo Next { get; set; }
}

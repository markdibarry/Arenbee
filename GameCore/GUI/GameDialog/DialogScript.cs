using System;
using System.Collections.Generic;

namespace GameCore.GUI.GameDialog;

public class DialogScript
{
    public string[] SpeakerIds { get; set; }
    public float[] InstFloats { get; set; }
    public string[] InstStrings { get; set; }
    public Choice[] Choices { get; set; }
    public int[][] ChoiceSets { get; set; }
    public Section[] Sections { get; set; }
    public LineData[] Lines { get; set; }
    public InstructionStatement[][] ConditionalSets { get; set; }
    public InstructionStatement[] InstructionStmts { get; set; }
    public int[][] Instructions { get; set; }
}

public enum StatementType
{
    Undefined,
    Line,
    Conditional,
    Instruction,
    Choice,
    Section
}

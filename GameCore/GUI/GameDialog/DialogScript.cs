using System.Text.Json.Serialization;

namespace GameCore.GUI.GameDialog;

public class DialogScript
{
    [JsonConstructor]
    public DialogScript(
        string[] speakerIds,
        float[] instFloats,
        string[] instStrings,
        Choice[] choices,
        ushort[][] choiceSets,
        Section[] sections,
        LineData[] lines,
        InstructionStatement[][] conditionalSets,
        InstructionStatement[] instructionStmts,
        ushort[][] instructions)
    {
        SpeakerIds = speakerIds;
        InstFloats = instFloats;
        InstStrings = instStrings;
        Choices = choices;
        ChoiceSets = choiceSets;
        Sections = sections;
        Lines = lines;
        ConditionalSets = conditionalSets;
        InstructionStmts = instructionStmts;
        Instructions = instructions;
    }

    public string[] SpeakerIds { get; set; }
    public float[] InstFloats { get; set; }
    public string[] InstStrings { get; set; }
    public Choice[] Choices { get; set; }
    public ushort[][] ChoiceSets { get; set; }
    public Section[] Sections { get; set; }
    public LineData[] Lines { get; set; }
    public InstructionStatement[][] ConditionalSets { get; set; }
    public InstructionStatement[] InstructionStmts { get; set; }
    public ushort[][] Instructions { get; set; }
}

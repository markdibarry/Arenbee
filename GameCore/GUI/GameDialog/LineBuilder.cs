using System.Text;

namespace GameCore.GUI.GameDialog;

public class LineBuilder
{
    public LineBuilder(LineData lineData, DialogScript dialogScript, LookupRegister lookupRegister)
    {
        DialogScript = dialogScript;
        LineData = lineData;
        _lookupRegister = lookupRegister;
    }

    private readonly LookupRegister _lookupRegister;
    public DialogScript DialogScript { get; set; }
    public LineData LineData { get; set; }
    public Line Line { get; set; } = new();
    public StringBuilder StringBuilder { get; set; } = new();
    public int RenderIndex { get; set; }
    public int TextIndex { get; set; }

    private Line ExtractEvents()
    {
        int appendStart = 0;

        while (TextIndex < LineData.Text.Length)
        {
            if (LineData.Text[TextIndex] != '[')
            {
                TextIndex++;
                RenderIndex++;
                continue;
            }

            if (TextIndex == 0 || LineData.Text[TextIndex - 1] != '\\')
            {
                StringBuilder.Append(LineData.Text[appendStart..TextIndex]);
                appendStart = TextIndex;
                int bracketLength = GetBracketLength(LineData.Text, TextIndex);
                if (!int.TryParse(LineData.Text[(TextIndex + 1)..(TextIndex + bracketLength)], out int intValue))
                    return null;
                int[] instr = DialogScript.Instructions[LineData.InstructionIndices[intValue]];
                HandleEventTag(instr);
                TextIndex += bracketLength;
            }
        }
    }

    private void HandleEventTag(int[] instructions)
    {
        switch ((InstructionType)instructions[0])
        {
            case InstructionType.BBCode:
                HandleBBCode(instructions);
                break;
            case InstructionType.NewLine:
                HandleNewLine();
                break;
            case InstructionType.Speed:
                HandleSpeed(instructions);
                break;
            case InstructionType.Goto:
                HandleGoTo(instructions);
                break;
            case InstructionType.Auto:
                HandleAuto(instructions);
                break;
            case InstructionType.Assign:
            case InstructionType.MultAssign:
            case InstructionType.DivAssign:
            case InstructionType.AddAssign:
            case InstructionType.SubAssign:
                HandleAssign(instructions);
                break;
            case InstructionType.Float:
            case InstructionType.String:
            case InstructionType.Bool:
            case InstructionType.Var:
            case InstructionType.Func:
            case InstructionType.Mult:
            case InstructionType.Div:
            case InstructionType.Add:
            case InstructionType.Sub:
            case InstructionType.LessEquals:
            case InstructionType.GreaterEquals:
            case InstructionType.Less:
            case InstructionType.Greater:
            case InstructionType.Equals:
            case InstructionType.NotEquals:
                HandleEvaluate(instructions);
                break;
        }
    }

    private void HandleAssign(int[] instructions)
    {

    }

    private void HandleEvaluate(int[] instructions)
    {

    }

    private void HandleAuto(int[] instructions)
    {
        Line.Auto = instructions[1] == 1;
    }

    private void HandleBBCode(int[] instructions)
    {
        StringBuilder.Append('[' + DialogScript.InstStrings[instructions[1]] + ']');
    }

    private void HandleNewLine()
    {
        StringBuilder.Append('\r');
    }

    private void HandleSpeed(int[] instructions)
    {
        SpeedTextEvent textEvent = new();
        textEvent.TimeMulitplier = DialogScript.InstFloats[instructions[1]];
        textEvent.Index = RenderIndex;
        Line.Events.Add(textEvent);
    }

    private void HandleGoTo(int[] instructions)
    {
        Line.Next = new GoTo(StatementType.Section, instructions[1]);
    }

    private static int GetBracketLength(string text, int i)
    {
        int length = 1;
        i++;
        while (i < text.Length)
        {
            if (text[i] == ']')
                return ++length;
            else if (text[i] == '[')
                return length;
            length++;
            i++;
        }
        return length;
    }
}

public class EventExtractor
{





}

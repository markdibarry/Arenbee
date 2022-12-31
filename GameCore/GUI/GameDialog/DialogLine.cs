using System.Collections.Generic;
using System.Text;

namespace GameCore.GUI.GameDialog;

public class DialogLine : IStatement
{
    public DialogLine(DialogInterpreter interpreter, DialogScript script, LineData lineData, Speaker[] globalSpeakers, bool auto)
    {
        Auto = auto;
        Speakers = globalSpeakers;
        _interpreter = interpreter;
        _script = script;
        _lineData = lineData;

        var lineSpeakers = new Speaker[_lineData.SpeakerIndices.Length];
        for (int i = 0; i < _lineData.SpeakerIndices.Length; i++)
            lineSpeakers[i] = globalSpeakers[_lineData.SpeakerIndices[i]];
        Speakers = lineSpeakers;
    }

    private readonly DialogInterpreter _interpreter;
    private readonly DialogScript _script;
    private readonly LineData _lineData;
    public bool Auto { get; private set; }
    public GoTo Next { get; private set; }
    public Speaker[] Speakers { get; }
    public string Text => _lineData.Text;

    public bool SameSpeakers(DialogLine secondLine) => Speaker.SameSpeakers(Speakers, secondLine.Speakers);

    public bool AnySpeakers(DialogLine secondLine) => Speaker.AnySpeakers(Speakers, secondLine.Speakers);

    public string GetEventParsedText(string bbCodeParsedText, List<TextEvent> events)
    {
        StringBuilder sb = new();
        int appendStart = 0;
        int renderedIndex = 0;
        int parsedIndex = 0;
        int i = 0;

        while (i < Text.Length)
        {
            if (Text[i] != '[' || (i != 0 && Text[i - 1] == '\\'))
            {
                i++;
                renderedIndex++;
                parsedIndex++;
                continue;
            }

            int bracketLength = GetBracketLength(Text, i);

            // If doesn't close, ignore
            if (Text[i + bracketLength - 1] != ']')
            {
                i += bracketLength;
                renderedIndex += bracketLength;
                parsedIndex += bracketLength;
                continue;
            }

            // is bbCode, so only increase Text index
            if (bbCodeParsedText[parsedIndex] != '[')
            {
                i += bracketLength;
                continue;
            }

            if (!TryAddTextEvent(Text[(i + 1)..(i + bracketLength)]))
            {
                i += bracketLength;
                renderedIndex += bracketLength;
                parsedIndex += bracketLength;
                continue;
            }

            sb.Append(Text[appendStart..i]);
            i += bracketLength;
            parsedIndex += bracketLength;
            appendStart = i;
        }

        sb.Append(Text[appendStart..i]);
        return sb.ToString();

        bool TryAddTextEvent(string tagContent)
        {
            if (!int.TryParse(tagContent, out int intValue))
                return false;
            ushort[] instructions = _script.Instructions[_lineData.InstructionIndices[intValue]];

            switch ((OpCode)instructions[0])
            {
                case OpCode.Assign:
                case OpCode.MultAssign:
                case OpCode.DivAssign:
                case OpCode.AddAssign:
                case OpCode.SubAssign:
                    AddAsTextEvent();
                    break;
                case OpCode.String:
                case OpCode.Float:
                case OpCode.Mult:
                case OpCode.Div:
                case OpCode.Add:
                case OpCode.Sub:
                case OpCode.Var:
                case OpCode.Func:
                    HandleEvaluate();
                    break;
                case OpCode.NewLine:
                    HandleNewLine();
                    break;
                case OpCode.Speed:
                    HandleSpeed();
                    break;
                case OpCode.Goto:
                    HandleGoTo();
                    break;
                case OpCode.Auto:
                    HandleAuto();
                    break;
                case OpCode.SpeakerSet:
                    HandleSpeakerSet();
                    break;
            };
            return true;

            void AddAsTextEvent()
            {
                InstructionTextEvent textEvent = new(renderedIndex, instructions);
                events.Add(textEvent);
            }

            void HandleEvaluate()
            {
                string result = string.Empty;
                switch (_interpreter.GetReturnType(instructions, 0))
                {
                    case VarType.String:
                        result = _interpreter.GetStringInstResult(instructions);
                        break;
                    case VarType.Float:
                        result = _interpreter.GetFloatInstResult(instructions).ToString();
                        break;
                    case VarType.Bool:
                        _interpreter.GetBoolInstResult(instructions);
                        break;
                    case VarType.Void:
                        AddAsTextEvent();
                        break;
                }
                sb.Append(result);
                renderedIndex += result.Length;
            }

            void HandleAuto() => Auto = instructions[1] == 1;

            void HandleNewLine()
            {
                sb.Append('\r');
                renderedIndex++;
            }

            void HandleSpeed()
            {
                SpeedTextEvent textEvent = new(renderedIndex, _script.InstFloats[instructions[1]]);
                events.Add(textEvent);
            }

            void HandleGoTo() => Next = new GoTo(StatementType.Section, instructions[1]);

            void HandleSpeakerSet()
            {
                int i = 1;
                string speakerId = _script.SpeakerIds[i++];
                string? displayName = null, portraitId = null, mood = null;
                if (instructions[i++] == 1)
                {
                    ushort[] nameInst = _script.Instructions[instructions[i++]];
                    displayName = _interpreter.GetStringInstResult(nameInst);
                }
                if (instructions[i++] == 1)
                {
                    ushort[] portraitInst = _script.Instructions[instructions[i++]];
                    portraitId = _interpreter.GetStringInstResult(portraitInst);
                }
                if (instructions[i++] == 1)
                {
                    ushort[] moodInst = _script.Instructions[instructions[i++]];
                    mood = _interpreter.GetStringInstResult(moodInst);
                }
                SpeakerTextEvent textEvent = new(renderedIndex, speakerId, displayName, portraitId, mood);
                events.Add(textEvent);
            }
        }

        static int GetBracketLength(string text, int i)
        {
            int length = 1;
            i++;
            while (i < text.Length)
            {
                if (text[i] == ']' && text[i - 1] != '\\')
                    return ++length;
                else if (text[i] == '[' && text[i - 1] != '\\')
                    return length;
                length++;
                i++;
            }
            return length;
        }
    }
}

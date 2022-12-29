using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.GUI.GameDialog;

public class DialogLine : IStatement, ITextLine
{
    public DialogLine(DialogInterpreter interpreter, DialogScript script, LineData lineData, Speaker[] speakers, bool auto)
    {
        Auto = auto;
        Speakers = speakers;
        StringBuilder stringBuilder = new();
        List<TextEvent> events = new();
        int appendStart = 0;
        int textIndex = 0;
        int renderIndex = 0;

        while (textIndex < lineData.Text.Length)
        {
            if (lineData.Text[textIndex] != '[')
            {
                textIndex++;
                renderIndex++;
                continue;
            }

            if (textIndex != 0 && lineData.Text[textIndex - 1] == '\\')
                continue;

            stringBuilder.Append(lineData.Text[appendStart..textIndex]);
            int bracketLength = GetBracketLength(lineData.Text, textIndex);
            if (bracketLength == -1)
                throw new Exception("Bracket length invalid.");
            if (!int.TryParse(lineData.Text[(textIndex + 1)..(textIndex + bracketLength)], out int intValue))
                throw new Exception("Bracket contains non-integer");
            ushort[] instr = script.Instructions[lineData.InstructionIndices[intValue]];
            HandleEventTag(instr);
            textIndex += bracketLength;
            appendStart = textIndex;
        }

        stringBuilder.Append(lineData.Text[appendStart..textIndex]);
        Events = events.ToArray();
        Text = stringBuilder.ToString();

        void HandleEventTag(ushort[] instructions)
        {
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
                case OpCode.BBCode:
                    HandleBBCode();
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

            void AddAsTextEvent()
            {
                InstructionTextEvent textEvent = new(renderIndex, instructions);
                events.Add(textEvent);
            }

            void HandleEvaluate()
            {
                string result = string.Empty;
                switch (interpreter.GetReturnType(instructions, 0))
                {
                    case VarType.String:
                        result = interpreter.GetStringInstResult(instructions);
                        break;
                    case VarType.Float:
                        result = interpreter.GetFloatInstResult(instructions).ToString();
                        break;
                    case VarType.Bool:
                        interpreter.GetBoolInstResult(instructions);
                        break;
                    case VarType.Void:
                        AddAsTextEvent();
                        break;
                }
                stringBuilder.Append(result);
                renderIndex += result.Length;
            }

            void HandleAuto() => Auto = instructions[1] == 1;

            void HandleBBCode()
            {
                stringBuilder.Append($"[{script.InstStrings[instructions[1]]}]");
            }

            void HandleNewLine()
            {
                stringBuilder.Append('\r');
                renderIndex++;
            }

            void HandleSpeed()
            {
                SpeedTextEvent textEvent = new(script.InstFloats[instructions[1]])
                {
                    Index = renderIndex
                };
                events.Add(textEvent);
            }

            void HandleGoTo() => Next = new GoTo(StatementType.Section, instructions[1]);

            void HandleSpeakerSet()
            {
                int i = 1;
                string speakerId = script.SpeakerIds[i++];
                string? displayName = null, portraitId = null, mood = null;
                if (instructions[i++] == 1)
                {
                    ushort[] nameInst = script.Instructions[instructions[i++]];
                    displayName = interpreter.GetStringInstResult(nameInst);
                }
                if (instructions[i++] == 1)
                {
                    ushort[] portraitInst = script.Instructions[instructions[i++]];
                    portraitId = interpreter.GetStringInstResult(portraitInst);
                }
                if (instructions[i++] == 1)
                {
                    ushort[] moodInst = script.Instructions[instructions[i++]];
                    mood = interpreter.GetStringInstResult(moodInst);
                }
                SpeakerTextEvent textEvent = new(speakerId, displayName, portraitId, mood)
                {
                    Index = renderIndex
                };
                events.Add(textEvent);
            }
        }

        static int GetBracketLength(string text, int i)
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
            return -1;
        }
    }

    public bool Auto { get; set; }
    public TextEvent[] Events { get; }
    public GoTo Next { get; set; }
    public Speaker[] Speakers { get; }
    public string Text { get; set; }

    public bool SameSpeakers(DialogLine secondLine) => Speaker.SameSpeakers(Speakers, secondLine.Speakers);

    public bool AnySpeakers(DialogLine secondLine) => Speaker.AnySpeakers(Speakers, secondLine.Speakers);
}

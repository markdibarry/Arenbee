using System.Text;

namespace GameCore.GUI.GameDialog;

public class LineBuilder
{
    public static DialogLine BuildLine(Evaluator evaluator, DialogScript dialogScript, LineData lineData)
    {
        DialogLine line = new();
        StringBuilder stringBuilder = new();
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

            if (textIndex == 0 || lineData.Text[textIndex - 1] != '\\')
            {
                stringBuilder.Append(lineData.Text[appendStart..textIndex]);
                int bracketLength = GetBracketLength(lineData.Text, textIndex);
                if (bracketLength == -1)
                    return null;
                if (!int.TryParse(lineData.Text[(textIndex + 1)..(textIndex + bracketLength)], out int intValue))
                    return null;
                int[] instr = dialogScript.Instructions[lineData.InstructionIndices[intValue]];
                HandleEventTag(instr);
                textIndex += bracketLength;
                appendStart = textIndex;
            }
        }
        stringBuilder.Append(lineData.Text[appendStart..textIndex]);

        line.Text = stringBuilder.ToString();
        return line;

        void HandleEventTag(int[] instructions)
        {
            switch ((InstructionType)instructions[0])
            {
                case InstructionType.Assign:
                case InstructionType.MultAssign:
                case InstructionType.DivAssign:
                case InstructionType.AddAssign:
                case InstructionType.SubAssign:
                    HandleDeferredEvaluate();
                    break;
                case InstructionType.String:
                case InstructionType.Float:
                case InstructionType.Mult:
                case InstructionType.Div:
                case InstructionType.Add:
                case InstructionType.Sub:
                case InstructionType.Var:
                case InstructionType.Func:
                    HandleEvaluate();
                    break;
                case InstructionType.BBCode:
                    HandleBBCode();
                    break;
                case InstructionType.NewLine:
                    HandleNewLine();
                    break;
                case InstructionType.Speed:
                    HandleSpeed();
                    break;
                case InstructionType.Goto:
                    HandleGoTo();
                    break;
                case InstructionType.Auto:
                    HandleAuto();
                    break;
            };

            void HandleDeferredEvaluate()
            {
                InstructionTextEvent textEvent = new()
                {
                    Index = renderIndex,
                    Instructions = instructions
                };
                line.Events.Add(textEvent);
            }

            void HandleEvaluate()
            {
                string result = string.Empty;
                switch (evaluator.GetReturnType(dialogScript, instructions, 0))
                {
                    case VarType.String:
                        result = evaluator.GetStringInstResult(dialogScript, instructions);
                        break;
                    case VarType.Float:
                        result = evaluator.GetFloatInstResult(dialogScript, instructions).ToString();
                        break;
                    case VarType.Void:
                        HandleDeferredEvaluate();
                        break;
                }
                stringBuilder.Append(result);
                renderIndex += result.Length;
            }

            void HandleAuto() => line.Auto = instructions[1] == 1;

            void HandleBBCode()
            {
                stringBuilder.Append('[' + dialogScript.InstStrings[instructions[1]] + ']');
            }

            void HandleNewLine()
            {
                stringBuilder.Append('\r');
                renderIndex++;
            }

            void HandleSpeed()
            {
                SpeedTextEvent textEvent = new()
                {
                    TimeMulitplier = dialogScript.InstFloats[instructions[1]],
                    Index = renderIndex
                };
                line.Events.Add(textEvent);
            }

            void HandleGoTo() => line.Next = new GoTo(StatementType.Section, instructions[1]);
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
}

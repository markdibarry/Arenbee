using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameCore.Exceptions;

namespace GameCore.GUI.GameDialog;

public class DialogScriptReader
{
    public DialogScriptReader(Dialog dialog, DialogBridgeRegister register, DialogScript dialogScript)
    {
        _dialog = dialog;
        _dialogScript = dialogScript;
        _interpreter = new(register, dialogScript, new());
        SetSpeakers();
    }

    private readonly DialogInterpreter _interpreter;
    private readonly Dialog _dialog;
    private readonly DialogScript _dialogScript;
    public List<Speaker> Speakers { get; set; } = new();

    public async Task ReadScriptAsync()
    {
        await HandleNextAsync(_dialogScript.Sections[0]);
    }

    public async Task HandleNextAsync(IStatement stmt)
    {
        switch (stmt.Next.Type)
        {
            case StatementType.Line:
                await HandleLineStatement();
                break;
            case StatementType.Section:
                await HandleSectionStatement();
                break;
            case StatementType.Instruction:
                HandleInstructionStatement();
                break;
            case StatementType.Conditional:
                await HandleConditionalStatement();
                break;
            case StatementType.Choice:
                HandleChoiceStatement();
                break;
        }

        async Task HandleLineStatement()
        {
            LineData lineData = _dialogScript.Lines[stmt.Next.Index];
            DialogLine line = BuildLine(_interpreter, _dialogScript, lineData);
            for (int i = 0; i < lineData.SpeakerIndices.Length; i++)
                line.Speakers.Add(Speakers[lineData.SpeakerIndices[i]]);
            await _dialog.HandleLineAsync(line);
        }

        async Task HandleSectionStatement()
        {
            await HandleNextAsync(_dialogScript.Sections[stmt.Next.Index]);
        }

        async void HandleInstructionStatement()
        {
            InstructionStatement instruction = _dialogScript.InstructionStmts[stmt.Next.Index];
        }

        async Task HandleConditionalStatement()
        {
            InstructionStatement[] conditions = _dialogScript.ConditionalSets[stmt.Next.Index];
            foreach (var condition in conditions)
            {
                if (condition.Values == null || _interpreter.GetBoolInstResult(condition.Values))
                {
                    await HandleNextAsync(condition);
                    break;
                }
            }
            throw new DialogException("No valid condition in if statement.");
        }

        void HandleChoiceStatement()
        {
            ushort[] choiceSet = _dialogScript.ChoiceSets[stmt.Next.Index];
        }
    }

    private void SetSpeakers()
    {
        foreach (string id in _dialogScript.SpeakerIds)
            Speakers.Add(new(id));
    }

    private static DialogLine BuildLine(DialogInterpreter evaluator, DialogScript dialogScript, LineData lineData)
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
                    throw new Exception("Bracket length invalid.");
                if (!int.TryParse(lineData.Text[(textIndex + 1)..(textIndex + bracketLength)], out int intValue))
                    throw new Exception("Bracket contains non-integer");
                ushort[] instr = dialogScript.Instructions[lineData.InstructionIndices[intValue]];
                HandleEventTag(instr);
                textIndex += bracketLength;
                appendStart = textIndex;
            }
        }
        stringBuilder.Append(lineData.Text[appendStart..textIndex]);

        line.Text = stringBuilder.ToString();
        return line;

        void HandleEventTag(ushort[] instructions)
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
                switch (evaluator.GetReturnType(instructions, 0))
                {
                    case VarType.String:
                        result = evaluator.GetStringInstResult(instructions);
                        break;
                    case VarType.Float:
                        result = evaluator.GetFloatInstResult(instructions).ToString();
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
                stringBuilder.Append($"[{dialogScript.InstStrings[instructions[1]]}]");
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

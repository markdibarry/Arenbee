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

    public void Evaluate(ushort[] instructions)
    {
        EvaluateInstructions(instructions);
    }

    public async Task ReadScriptAsync()
    {
        await ReadNextStatementAsync(_dialogScript.Sections[0].Next);
    }

    public async Task ReadNextStatementAsync(GoTo goTo)
    {
        switch (goTo.Type)
        {
            case StatementType.Line:
                await HandleLineStatement();
                break;
            case StatementType.Section:
                await HandleSectionStatement();
                break;
            case StatementType.Instruction:
                await HandleInstructionStatement();
                break;
            case StatementType.Conditional:
                await HandleConditionalStatement();
                break;
            case StatementType.Choice:
                await HandleChoiceStatement();
                break;
        }

        async Task HandleLineStatement()
        {
            LineData lineData = _dialogScript.Lines[goTo.Index];
            DialogLine line = BuildLine(lineData);
            for (int i = 0; i < lineData.SpeakerIndices.Length; i++)
                line.Speakers.Add(Speakers[lineData.SpeakerIndices[i]]);
            await _dialog.HandleLineAsync(line);
        }

        async Task HandleSectionStatement()
        {
            await ReadNextStatementAsync(_dialogScript.Sections[goTo.Index].Next);
        }

        async Task HandleInstructionStatement()
        {
            InstructionStatement instructionStmt = _dialogScript.InstructionStmts[goTo.Index];
            GoTo next = EvaluateInstructions(instructionStmt.Values);
            if (next.Type == StatementType.Undefined)
                next = instructionStmt.Next;
            await ReadNextStatementAsync(next);
        }

        async Task HandleConditionalStatement()
        {
            InstructionStatement[] conditions = _dialogScript.ConditionalSets[goTo.Index];
            foreach (var condition in conditions)
            {
                if (condition.Values.Length == 0 || _interpreter.GetBoolInstResult(condition.Values))
                {
                    await ReadNextStatementAsync(condition.Next);
                    return;
                }
            }
            throw new DialogException("No valid condition in if statement.");
        }

        async Task HandleChoiceStatement()
        {
            List<Choice> choices = new();
            ushort[] choiceSet = _dialogScript.ChoiceSets[goTo.Index];
            int validIndex = -1;
            for (int i = 0; i < choiceSet.Length; i++)
            {
                if (i >= validIndex)
                    validIndex = -1;
                // If choice, add it!
                if (choiceSet[i] == (ushort)OpCode.Choice)
                {
                    Choice choice = _dialogScript.Choices[choiceSet[++i]];
                    choice.Disabled = i < validIndex;
                    choices.Add(choice);
                }
                // If GoTo, flag all choices as disabled until index
                else if (choiceSet[i] == (ushort)OpCode.Goto)
                {
                    if (i++ < validIndex)
                        validIndex = choiceSet[i];
                }
                // Otherwise is a condition
                else
                {
                    if (i < validIndex)
                    {
                        i++;
                    }
                    else
                    {
                        ushort[] condition = _dialogScript.Instructions[choiceSet[i++]];
                        if (!_interpreter.GetBoolInstResult(condition))
                            validIndex = choiceSet[i];
                    }
                }
            }
            await _dialog.OpenOptionBoxAsync(choices);
        }
    }

    private void SetSpeakers()
    {
        foreach (string id in _dialogScript.SpeakerIds)
            Speakers.Add(new(id));
    }

    private DialogLine BuildLine(LineData lineData)
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
                ushort[] instr = _dialogScript.Instructions[lineData.InstructionIndices[intValue]];
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
                line.Events.Add(textEvent);
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
                stringBuilder.Append(result);
                renderIndex += result.Length;
            }

            void HandleAuto() => line.Auto = instructions[1] == 1;

            void HandleBBCode()
            {
                stringBuilder.Append($"[{_dialogScript.InstStrings[instructions[1]]}]");
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
                    TimeMulitplier = _dialogScript.InstFloats[instructions[1]],
                    Index = renderIndex
                };
                line.Events.Add(textEvent);
            }

            void HandleGoTo() => line.Next = new GoTo(StatementType.Section, instructions[1]);

            void HandleSpeakerSet()
            {

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

    private GoTo EvaluateInstructions(ushort[] instructions)
    {
        switch ((OpCode)instructions[0])
        {
            case OpCode.Assign:
            case OpCode.MultAssign:
            case OpCode.DivAssign:
            case OpCode.AddAssign:
            case OpCode.SubAssign:
            case OpCode.Func:
                HandleEvaluate();
                break;
            case OpCode.Speed:
                HandleSpeed();
                break;
            case OpCode.Goto:
                return HandleGoTo();
            case OpCode.Auto:
                HandleAuto();
                break;
            case OpCode.SpeakerSet:
                HandleSpeakerSet();
                break;
        };
        return new GoTo();
        void HandleEvaluate()
        {
            switch (_interpreter.GetReturnType(instructions, 0))
            {
                case VarType.String:
                    _interpreter.GetStringInstResult(instructions);
                    break;
                case VarType.Float:
                    _interpreter.GetFloatInstResult(instructions).ToString();
                    break;
                case VarType.Bool:
                    _interpreter.GetBoolInstResult(instructions);
                    break;
                case VarType.Void:
                    _interpreter.EvalVoidInst(instructions);
                    break;
            }
        }

        void HandleSpeed()
        {
        }

        GoTo HandleGoTo() => new(StatementType.Section, instructions[1]);

        void HandleAuto()
        {

        }

        void HandleSpeakerSet()
        {

        }
    }
}

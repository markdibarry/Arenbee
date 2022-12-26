using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameCore.Exceptions;

namespace GameCore.GUI.GameDialog;

public class DialogScriptReader
{
    public DialogScriptReader(Dialog dialog, DialogBridgeRegister register, DialogScript dialogScript)
    {
        _dialog = dialog;
        _dialogScript = dialogScript;
        _speakers = _dialogScript.SpeakerIds.Select(x => new Speaker(x)).ToArray();
        _interpreter = new(register, dialogScript, new());
    }

    private readonly DialogInterpreter _interpreter;
    private readonly Dialog _dialog;
    private readonly DialogScript _dialogScript;
    private readonly Speaker[] _speakers;

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
            Speaker[] lineSpeakers = GetSpeakers(lineData.SpeakerIndices);
            DialogLine line = new(_interpreter, _dialogScript, lineData, lineSpeakers);
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

    private Speaker[] GetSpeakers(ushort[] indices)
    {
        if (indices.Length == 0)
            return Array.Empty<Speaker>();
        var lineSpeakers = new Speaker[indices.Length];
        for (int i = 0; i < indices.Length; i++)
            lineSpeakers[i] = _speakers[indices[i]];
        return lineSpeakers;
    }
}

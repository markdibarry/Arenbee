using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.GUI.GameDialog;

public class DialogScriptReader
{
    public DialogScriptReader(Dialog dialog)
    {
        _dialog = dialog;
    }

    private readonly Dialog _dialog;
    private DialogScript _currentScript;
    public List<Speaker> Speakers { get; set; }

    public void ReadScript(DialogScript dialogScript)
    {
        _currentScript = dialogScript;
        SetSpeakers();
        HandleNext(_currentScript.Sections[0]);
    }

    public void HandleLineStatement(int index)
    {
        LineData lineData = _currentScript.Lines[index];

        _dialog.HandleLine(lineData);
    }

    private void HandleSectionStatement(int index)
    {
        HandleNext(_currentScript.Sections[index]);
    }

    private void HandleInstructionStatement(int index)
    {
        InstructionStatement instruction = _currentScript.InstructionStmts[index];
    }

    private void HandleConditionalStatement(int index)
    {
        InstructionStatement[] conditions = _currentScript.ConditionalSets[index];
    }

    private void HandleChoiceStatement(int index)
    {
        int[] choiceSet = _currentScript.ChoiceSets[index];
    }

    private void HandleNext(IStatement stmt)
    {
        switch (stmt.Next.Type)
        {
            case StatementType.Line:
                HandleLineStatement(stmt.Next.Index);
                break;
            case StatementType.Section:
                HandleSectionStatement(stmt.Next.Index);
                break;
            case StatementType.Instruction:
                HandleInstructionStatement(stmt.Next.Index);
                break;
            case StatementType.Conditional:
                HandleConditionalStatement(stmt.Next.Index);
                break;
            case StatementType.Choice:
                HandleChoiceStatement(stmt.Next.Index);
                break;
        }
    }

    private void SetSpeakers()
    {
        foreach (string id in _currentScript.SpeakerIds)
            Speakers.Add(new(id));
    }
}

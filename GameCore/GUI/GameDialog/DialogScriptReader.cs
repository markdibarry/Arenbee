using System.Collections.Generic;

namespace GameCore.GUI.GameDialog;

public class DialogScriptReader
{
    public DialogScriptReader(Dialog dialog, DialogBridgeRegister register)
    {
        _dialog = dialog;
        _register = register;
    }

    private readonly Dialog _dialog;
    private readonly DialogBridgeRegister _register;
    private DialogScript _currentScript;
    public List<Speaker> Speakers { get; set; }

    public void ReadScript(DialogScript dialogScript)
    {
        _currentScript = dialogScript;
        SetSpeakers();
        HandleNext(_currentScript.Sections[0]);
    }

    public void HandleNext(IStatement stmt)
    {
        switch (stmt.Next.Type)
        {
            case StatementType.Line:
                HandleLineStatement();
                break;
            case StatementType.Section:
                HandleSectionStatement();
                break;
            case StatementType.Instruction:
                HandleInstructionStatement();
                break;
            case StatementType.Conditional:
                HandleConditionalStatement();
                break;
            case StatementType.Choice:
                HandleChoiceStatement();
                break;
        }

        void HandleLineStatement()
        {
            LineData lineData = _currentScript.Lines[stmt.Next.Index];
            DialogLine line = _register.BuildLine(_currentScript, lineData);
            for (int i = 0; i < lineData.SpeakerIndices.Length; i++)
                line.Speakers.Add(Speakers[lineData.SpeakerIndices[i]]);
            _dialog.HandleLineAsync(line);
        }

        void HandleSectionStatement()
        {
            HandleNext(_currentScript.Sections[stmt.Next.Index]);
        }

        void HandleInstructionStatement()
        {
            InstructionStatement instruction = _currentScript.InstructionStmts[stmt.Next.Index];
        }

        void HandleConditionalStatement()
        {
            InstructionStatement[] conditions = _currentScript.ConditionalSets[stmt.Next.Index];
        }

        void HandleChoiceStatement()
        {
            int[] choiceSet = _currentScript.ChoiceSets[stmt.Next.Index];
        }
    }

    private void SetSpeakers()
    {
        foreach (string id in _currentScript.SpeakerIds)
            Speakers.Add(new(id));
    }
}

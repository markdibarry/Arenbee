using System;
using System.Collections.Generic;

namespace GameCore.GUI.GameDialog;

public class DialogInterpreter
{
    public DialogInterpreter(DialogBridgeRegister register, DialogScript dialogScript, TextStorage textStorage)
    {
        _register = register;
        _dialogScript = dialogScript;
        _textStorage = textStorage;
    }

    private readonly DialogBridgeRegister _register;
    private readonly DialogScript _dialogScript;
    private readonly TextStorage _textStorage;
    private ushort[] _instructions = Array.Empty<ushort>();
    private int _index = -1;
    public enum InstructionType
    {
        Undefined,
        Float,
        String,
        Bool,
        // name string
        Var,
        // name string, number of arguments (float), expressions...
        Func,
        // float, float
        Mult,
        Div,
        Add,
        Sub,
        LessEquals,
        GreaterEquals,
        Less,
        Greater,
        // expression, expression
        Equals,
        NotEquals,
        // bool, bool
        And,
        Or,
        Not,
        // Variable index, expression
        Assign,
        MultAssign,
        DivAssign,
        AddAssign,
        SubAssign,

        // bool
        Auto,
        // string
        BBCode,
        // Section Index
        Goto,
        NewLine,
        // float
        Speed,
        // SpeakerId (float), Name (expression), Mood (expression), Portrait (expression)
        SpeakerSet
    }

    public VarType GetReturnType(ushort[] instructions, int index)
    {
        return (InstructionType)instructions[index + 1] switch
        {
            InstructionType.String => VarType.String,
            InstructionType.Float or
            InstructionType.Mult or
            InstructionType.Div or
            InstructionType.Add or
            InstructionType.Sub => VarType.Float,
            InstructionType.Bool or
            InstructionType.Less or
            InstructionType.Greater or
            InstructionType.LessEquals or
            InstructionType.GreaterEquals or
            InstructionType.Equals or
            InstructionType.NotEquals or
            InstructionType.Not => VarType.Bool,
            InstructionType.Func => GetFuncReturnType(),
            InstructionType.Var => GetVarType(),
            _ => default
        };

        VarType GetVarType()
        {
            string varName = _dialogScript.InstStrings[instructions[index + 2]];
            if (_register.Properties.TryGetValue(varName, out VarDef? varDef))
                return varDef.VarType;
            if (_textStorage == null || !_textStorage.TryGetValue(varName, out object? obj))
                return default;
            return obj switch
            {
                string => VarType.String,
                bool => VarType.Bool,
                float => VarType.Float,
                _ => default
            };
        }

        VarType GetFuncReturnType()
        {
            string funcName = _dialogScript.InstStrings[instructions[index + 2]];
            if (!_register.Methods.TryGetValue(funcName, out FuncDef? funcDef))
                return default;
            return funcDef.ReturnType;
        }
    }

    public float GetFloatInstResult(ushort[] instructions)
    {
        _instructions = instructions;
        float result = EvalFloatExp();
        _instructions = Array.Empty<ushort>();
        _index = -1;
        return result;
    }

    public bool GetBoolInstResult(ushort[] instructions)
    {
        _instructions = instructions;
        bool result = EvalBoolExp();
        _instructions = Array.Empty<ushort>();
        _index = -1;
        return result;
    }

    public string GetStringInstResult(ushort[] instructions)
    {
        _instructions = instructions;
        string result = EvalStringExp();
        _instructions = Array.Empty<ushort>();
        _index = -1;
        return result;
    }

    public void EvalAssignInst(ushort[] instructions)
    {
        _instructions = instructions;
        EvalVoidExp();
        _instructions = Array.Empty<ushort>();
        _index = -1;
    }

    private bool EvalBoolExp()
    {
        return (InstructionType)_instructions[++_index] switch
        {
            InstructionType.Bool => EvalBool(),
            InstructionType.Less => EvalLess(),
            InstructionType.Greater => EvalGreater(),
            InstructionType.LessEquals => EvalLessEquals(),
            InstructionType.GreaterEquals => EvalGreaterEquals(),
            InstructionType.Equals => EvalEquals(),
            InstructionType.NotEquals => !EvalEquals(),
            InstructionType.Not => EvalNot(),
            InstructionType.And => EvalAnd(),
            InstructionType.Or => EvalOr(),
            InstructionType.Var => EvalVar<bool>(),
            InstructionType.Func => (bool?)EvalFunc() ?? default,
            _ => default
        };
    }

    private string EvalStringExp()
    {
        return (InstructionType)_instructions[++_index] switch
        {
            InstructionType.String => EvalString(),
            InstructionType.Var => EvalVar<string>() ?? string.Empty,
            InstructionType.Func => (string?)EvalFunc() ?? string.Empty,
            _ => string.Empty
        };
    }

    private float EvalFloatExp()
    {
        return (InstructionType)_instructions[++_index] switch
        {
            InstructionType.Float => EvalFloat(),
            InstructionType.Mult => EvalMult(),
            InstructionType.Div => EvalDiv(),
            InstructionType.Add => EvalAdd(),
            InstructionType.Sub => EvalSub(),
            InstructionType.Var => EvalVar<float>(),
            InstructionType.Func => (float?)EvalFunc() ?? default,
            _ => default
        };
    }

    private void EvalVoidExp()
    {
        switch ((InstructionType)_instructions[++_index])
        {
            case InstructionType.Assign:
                EvalAssign();
                break;
            case InstructionType.MultAssign:
            case InstructionType.DivAssign:
            case InstructionType.AddAssign:
            case InstructionType.SubAssign:
                EvalMathAssign((InstructionType)_instructions[_index]);
                break;
            case InstructionType.Func:
                EvalFunc();
                break;
        };
        return;
    }

    private bool EvalEquals()
    {
        return GetReturnType(_instructions, _index) switch
        {
            VarType.Float => EvalFloatExp() == EvalFloatExp(),
            VarType.Bool => EvalBoolExp() == EvalBoolExp(),
            VarType.String => EvalStringExp() == EvalStringExp(),
            _ => default
        };
    }

    private bool EvalLess() => EvalFloatExp() < EvalFloatExp();

    private bool EvalGreater() => EvalFloatExp() > EvalFloatExp();

    private bool EvalLessEquals() => EvalFloatExp() <= EvalFloatExp();

    private bool EvalGreaterEquals() => EvalFloatExp() >= EvalFloatExp();

    private bool EvalAnd() => EvalBoolExp() && EvalBoolExp();

    private bool EvalOr() => EvalBoolExp() || EvalBoolExp();

    private bool EvalNot() => !EvalBoolExp();

    private float EvalMult() => EvalFloatExp() * EvalFloatExp();

    private float EvalDiv() => EvalFloatExp() / EvalFloatExp();

    private float EvalAdd() => EvalFloatExp() + EvalFloatExp();

    private float EvalSub() => EvalFloatExp() - EvalFloatExp();

    private bool EvalBool() => _instructions[++_index] == 1;

    private float EvalFloat() => _dialogScript.InstFloats[_instructions[++_index]];

    private string EvalString() => _dialogScript.InstStrings[_instructions[++_index]];

    private void EvalAssign()
    {
        string varName = _dialogScript.InstStrings[_instructions[++_index]];
        if (!_register.Properties.TryGetValue(varName, out VarDef? varDef))
            return;
        switch (varDef.VarType)
        {
            case VarType.Float:
                ((Action<float>)varDef.Setter).Invoke(EvalFloatExp());
                break;
            case VarType.Bool:
                ((Action<bool>)varDef.Setter).Invoke(EvalBoolExp());
                break;
            case VarType.String:
                ((Action<string>)varDef.Setter).Invoke(EvalStringExp());
                break;
        };
    }

    private void EvalMathAssign(InstructionType instructionType)
    {
        string varName = _dialogScript.InstStrings[_instructions[++_index]];
        if (!_register.Properties.TryGetValue(varName, out VarDef? varDef))
            return;
        float originalValue = ((Func<float>)varDef.Getter).Invoke();
        float result = instructionType switch
        {
            InstructionType.AddAssign => originalValue + EvalFloatExp(),
            InstructionType.SubAssign => originalValue - EvalFloatExp(),
            InstructionType.MultAssign => originalValue * EvalFloatExp(),
            InstructionType.DivAssign => originalValue / EvalFloatExp(),
            _ => default
        };
        ((Action<float>)varDef.Setter).Invoke(result);
    }

    private T? EvalVar<T>()
    {
        string varName = _dialogScript.InstStrings[_instructions[++_index]];
        if (_register.Properties.TryGetValue(varName, out VarDef? varDef))
            return ((Func<T>)varDef.Getter).Invoke();
        if (!_textStorage.TryGetValue(varName, out object? obj) || obj is not T)
            return default;
        return (T)obj;
    }

    private object? EvalFunc()
    {
        string funcName = _dialogScript.InstStrings[_instructions[++_index]];
        if (!_register.Methods.TryGetValue(funcName, out FuncDef? funcDef))
            return default;
        int argNum = _instructions[++_index];
        if (argNum == 0)
            return funcDef.Method.Invoke(Array.Empty<object>());
        List<object> args = new();
        for (int i = 0; i < argNum; i++)
        {
            switch (funcDef.ArgTypes[i])
            {
                case VarType.Float:
                    args.Add(EvalFloatExp());
                    break;
                case VarType.Bool:
                    args.Add(EvalBoolExp());
                    break;
                case VarType.String:
                    args.Add(EvalStringExp());
                    break;
            };
        }
        return funcDef.Method.Invoke(args.ToArray());
    }
}

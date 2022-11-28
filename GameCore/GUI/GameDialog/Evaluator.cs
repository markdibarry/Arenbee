namespace GameCore.GUI.GameDialog;

public class Evaluator
{
    public Evaluator(DialogScript dialogScript)
    {
        _dialogScript = dialogScript;
    }

    private readonly DialogScript _dialogScript;

    public object Evaluate(int[] arr)
    {
        if (arr == null || arr.Length == 0)
            return null;
        int index = 0;
        switch (GetReturnType(index, arr))
        {
            case VarType.Float:
                float floatVal = EvalFloatExp();
                break;
            case VarType.Bool:
                bool boolVal = EvalBoolExp();
                break;
            case VarType.String:
                string stringVal = EvalStringExp();
                break;
        };
    }

    public VarType GetReturnType(int index, int[] exp)
    {
        return (InstructionType)exp[index] switch
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
            InstructionType.Not or
            InstructionType.And or
            InstructionType.Or => VarType.Bool,
            //InstructionType.Var or
            //InstructionType.Func => _script.Variables[_expression[index + 1]].Type,
            _ => VarType.Undefined
        };
    }
}

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

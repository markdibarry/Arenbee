using System;
using System.Collections.Generic;

namespace GameCore.GUI.GameDialog;

public class VarDef
{
    public VarDef(Delegate getter, Delegate setter, VarType varType)
    {
        Getter = getter;
        Setter = setter;
        VarType = varType;
    }

    public VarType VarType { get; }
    public Delegate Getter { get; }
    public Delegate Setter { get; }
}

public class FuncDef
{
    public FuncDef(Func<object[], object?> method, VarType returnType, List<VarType> argTypes)
    {
        Method = method;
        ReturnType = returnType;
        ArgTypes = argTypes.ToArray();
    }

    public VarType ReturnType { get; }
    public Func<object[], object?> Method { get; }
    public VarType[] ArgTypes { get; }
}

public enum VarType
{
    Undefined = 0,
    Float = 1,
    String = 2,
    Bool = 3,
    Void = 4
}

public enum StatementType
{
    Undefined,
    Line,
    Conditional,
    Instruction,
    Choice,
    Section
}

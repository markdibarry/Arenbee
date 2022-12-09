using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GameCore.GUI.GameDialog;

public class DialogBridgeRegister
{
    public DialogBridgeRegister(DialogBridgeBase dialogBridgeBase)
    {
        _bridge = dialogBridgeBase;
        Evaluator = new(this);
        Properties = new(GenerateProperties());
        Methods = new(GenerateMethods());
    }

    public Evaluator Evaluator { get; }
    private readonly DialogBridgeBase _bridge;
    public ReadOnlyDictionary<string, VarDef> Properties { get; }
    public ReadOnlyDictionary<string, FuncDef> Methods { get; }

    public DialogLine BuildLine(DialogScript dialogScript, LineData lineData)
    {
        return LineBuilder.BuildLine(Evaluator, dialogScript, lineData);
    }

    private Dictionary<string, VarDef> GenerateProperties()
    {
        Dictionary<string, VarDef> properties = new();
        var propertyInfos = _bridge.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var propertyInfo in propertyInfos)
        {
            var getter = propertyInfo.GetGetMethod().CreateDelegate(GetDelegateType(propertyInfo.GetGetMethod()), _bridge);
            var setter = propertyInfo.GetSetMethod().CreateDelegate(GetDelegateType(propertyInfo.GetSetMethod()), _bridge);
            properties.Add(propertyInfo.Name, new(getter, setter, GetVarType(propertyInfo.PropertyType)));
        }
        return properties;
    }

    private Dictionary<string, FuncDef> GenerateMethods()
    {
        Dictionary<string, FuncDef> methods = new();
        var methodInfos = _bridge.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(x => !x.IsSpecialName);
        foreach (var methodInfo in methodInfos)
        {
            var del = CreateDynamicDelegate(methodInfo, _bridge);
            List<VarType> argTypes = new();
            foreach (var paramInfo in methodInfo.GetParameters())
                argTypes.Add(GetVarType(paramInfo.ParameterType));
            FuncDef funcDef = new(del, GetVarType(methodInfo.ReturnType), argTypes);
            methods.Add(methodInfo.Name, funcDef);
        }

        return methods;
    }

    private static VarType GetVarType(Type type)
    {
        if (type == typeof(float))
            return VarType.Float;
        else if (type == typeof(bool))
            return VarType.Bool;
        else if (type == typeof(string))
            return VarType.String;
        else if (type == typeof(void))
            return VarType.Void;
        return VarType.Undefined;
    }

    private static Type GetDelegateType(MethodInfo methodInfo)
    {
        return Expression.GetDelegateType(methodInfo.GetParameters()
            .Select(x => x.ParameterType)
            .Concat(new[] { methodInfo.ReturnType })
            .ToArray());
    }

    private static Expression[] CreateParams(MethodInfo methodInfo, Expression args)
    {
        return methodInfo.GetParameters()
            .Select((param, index) =>
                Expression.Convert(
                    Expression.ArrayIndex(args, Expression.Constant(index)),
                    param.ParameterType))
            .Cast<Expression>()
            .ToArray();
    }

    public static Func<object[], object> CreateDynamicDelegate(MethodInfo methodInfo, object target)
    {
        ParameterExpression argParams = Expression.Parameter(typeof(object[]), "args");
        MethodCallExpression call = Expression.Call(
            Expression.Constant(target, target.GetType()),
            methodInfo,
            CreateParams(methodInfo, argParams));

        if (methodInfo.ReturnType != typeof(void))
        {
            var lambda = Expression.Lambda<Func<object[], object>>(
                Expression.Convert(call, typeof(object)),
                argParams);
            return lambda.Compile();
        }
        else
        {
            var lambda = Expression.Lambda<Action<object[]>>(call, argParams);
            var compiled = lambda.Compile();
            return (parameters) =>
            {
                compiled(parameters);
                return null;
            };
        }
    }
}

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
    public FuncDef(Func<object[], object> method, VarType returnType, List<VarType> argTypes)
    {
        Method = method;
        ReturnType = returnType;
        ArgTypes = argTypes.ToArray();
    }

    public VarType ReturnType { get; }
    public Func<object[], object> Method { get; }
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

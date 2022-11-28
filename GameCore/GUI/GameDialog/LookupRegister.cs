using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GameCore.GUI.GameDialog;

public class LookupRegister
{
    public LookupRegister(DialogBridgeBase dialogBridgeBase)
    {
        _bridge = dialogBridgeBase;
        Properties = new(GenerateProperties());
        Methods = new(GenerateMethods());
    }

    private readonly DialogBridgeBase _bridge;
    public ReadOnlyDictionary<string, VarDef> Properties { get; }
    public ReadOnlyDictionary<string, FuncDef> Methods { get; }

    private Dictionary<string, VarDef> GenerateProperties()
    {
        Dictionary<string, VarDef> properties = new();
        var propertyInfos = _bridge.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var propertyInfo in propertyInfos)
        {
            var getter = propertyInfo.GetGetMethod().CreateDelegate(GetDelegateType(propertyInfo.GetGetMethod()), _bridge);
            var setter = propertyInfo.GetSetMethod().CreateDelegate(GetDelegateType(propertyInfo.GetSetMethod()), _bridge);
            properties.Add(propertyInfo.Name, new(getter, setter, propertyInfo.PropertyType));
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
            methods.Add(methodInfo.Name, new(CreateDynamicDelegate(methodInfo, _bridge), methodInfo.ReturnType));
        return methods;
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
    public VarDef(Delegate getter, Delegate setter, Type type)
    {
        Getter = getter;
        Setter = setter;
        Type = type;
    }

    public Type Type { get; }
    public Delegate Getter { get; }
    public Delegate Setter { get; }
}

public class FuncDef
{
    public FuncDef(Func<object[], object> method, Type type)
    {
        Method = method;
        Type = type;
    }

    public Type Type { get; }
    public Func<object[], object> Method { get; }
}

public enum VarType
{
    Undefined = 0,
    Float = 1,
    String = 2,
    Bool = 3,
    Void = 4
}

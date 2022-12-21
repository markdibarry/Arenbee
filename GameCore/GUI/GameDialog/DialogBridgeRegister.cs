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
        Properties = new(GenerateProperties());
        Methods = new(GenerateMethods());
    }

    private readonly DialogBridgeBase _bridge;
    public ReadOnlyDictionary<string, VarDef> Properties { get; }
    public ReadOnlyDictionary<string, FuncDef> Methods { get; }

    private Dictionary<string, VarDef> GenerateProperties()
    {
        Dictionary<string, VarDef> properties = new();
        PropertyInfo[] propertyInfos = _bridge.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
            MethodInfo getMethodInfo = propertyInfo.GetGetMethod()!;
            MethodInfo setMethodInfo = propertyInfo.GetSetMethod()!;
            var getter = getMethodInfo.CreateDelegate(GetDelegateType(getMethodInfo), _bridge);
            var setter = setMethodInfo.CreateDelegate(GetDelegateType(setMethodInfo), _bridge);
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

    public static Func<object[], object?> CreateDynamicDelegate(MethodInfo methodInfo, object target)
    {
        ParameterExpression argParams = Expression.Parameter(typeof(object[]), "args");
        MethodCallExpression call = Expression.Call(
            Expression.Constant(target, target.GetType()),
            methodInfo,
            CreateParams(methodInfo, argParams));

        if (methodInfo.ReturnType != typeof(void))
        {
            var lambda = Expression.Lambda<Func<object[], object?>>(
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

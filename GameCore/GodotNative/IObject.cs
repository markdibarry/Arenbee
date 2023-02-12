using System;
using Godot;

namespace GameCore.GodotNative;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable IDE1006 // Naming Styles
public interface IObject
{
    event Action PropertyListChanged;
    event Action ScriptChanged;

    Variant Call(StringName method, params Variant[] args);
    Variant CallDeferred(StringName method, params Variant[] args);
    Variant Callv(StringName method, Godot.Collections.Array argArray);
    Error Connect(StringName signal, Callable callable, uint flags = 0);
    void Disconnect(StringName signal, Callable callable);
    void Dispose();
    Error EmitSignal(StringName signal, params Variant[] args);
    void Free();
    bool IsConnected(StringName signal, Callable callable);
    void Notification(int what, bool reversed = false);
    void SetDeferred(StringName property, Variant value);
    SignalAwaiter ToSignal(GodotObject source, StringName signal);
    string Tr(StringName message, StringName context = null);
    string TrN(StringName message, StringName pluralMessage, int n, StringName context = null);
    void _Notification(int what);
}
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

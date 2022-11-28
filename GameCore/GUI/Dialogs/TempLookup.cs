using System.Collections.Generic;

namespace GameCore.GUI;

public class TempLookup : ILookupContext
{
    public TempLookup()
    {
        SetValue("greeting", "Salutations!");
        SetValue("coolTitle", "the G.O.A.T.");
    }
    private readonly Dictionary<string, object> _tempStorage = new();

    public void Clear()
    {
        _tempStorage.Clear();
    }

    public bool Contains(string key)
    {
        return _tempStorage.ContainsKey(key);
    }

    public void SetValue(string key, string value)
    {
        SetValue<string>(key, value);
    }

    public void SetValue(string key, float value)
    {
        SetValue<float>(key, value);
    }

    public void SetValue(string key, bool value)
    {
        SetValue<bool>(key, value);
    }

    public bool TryGetValueAsString(string key, out string result)
    {
        if (!Contains(key))
        {
            result = default;
            return false;
        }

        result = _tempStorage[key].ToString();
        return true;
    }

    public bool TryGetValue<T>(string key, out T result)
    {
        if (!_tempStorage.TryGetValue(key, out object val) || val is not T)
        {
            result = default(T);
            return false;
        }

        result = (T)val;
        return true;
    }

    private void SetValue<T>(string key, T value)
    {
        if (!Contains(key))
        {
            _tempStorage[key] = value;
            return;
        }
        if (_tempStorage[key] is T)
            _tempStorage[key] = value;
    }
}

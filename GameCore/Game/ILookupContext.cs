namespace Arenbee;

public interface ILookupContext
{
    void Clear();
    bool Contains(string key);
    void SetValue(string key, string stringValue);
    void SetValue(string key, float floatValue);
    void SetValue(string key, bool boolValue);
    bool TryGetValue<T>(string key, out T result);
    bool TryGetValueAsString(string key, out string result);
}

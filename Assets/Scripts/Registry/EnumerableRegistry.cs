using System;

public abstract class EnumerableRegistry<K, V> : AbstractRegistry<string, V>
where K : struct, IConvertible
{
    public EnumerableRegistry()
    {
        if (!typeof(K).IsEnum)
        {
            throw new ArgumentException("Key parameter must be an enumerated type");
        }
    }

    // Get from registry by key.
    public V Get(K key)
    {
        string name = Enum.GetName(typeof(K), key);
        return base.Get(name);
    }

    // Set registry value by key.
    public void Set(K key, V value)
    {
        string name = Enum.GetName(typeof(K), key);
        base.Set(name, value);
    }
}
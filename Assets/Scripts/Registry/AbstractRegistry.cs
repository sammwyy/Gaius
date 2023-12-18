using System;
using System.Collections.Generic;

public abstract class AbstractRegistry<K, V>
{
    // Registry.
    protected Dictionary<K, V> _registry = new();

    // Get from registry by key.
    public V Get(K key)
    {
        return this._registry[key];
    }

    // Set registry value by key.
    public void Set(K key, V value)
    {
        this._registry[key] = value;
    }

    // Count registry items.
    public int Count()
    {
        return this._registry.Count;
    }

    // Get all registry keys.
    public K[] GetKeys()
    {
        K[] keys = new K[this._registry.Count];
        this._registry.Keys.CopyTo(keys, 0);
        return keys;
    }

    // Get all registry values.
    public V[] GetValues()
    {
        V[] values = new V[this._registry.Count];
        this._registry.Values.CopyTo(values, 0);
        return values;
    }

    // Get all registry items.
    public Dictionary<K, V> GetItems()
    {
        return this._registry;
    }
}
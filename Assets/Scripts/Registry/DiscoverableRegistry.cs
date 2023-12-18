public class DiscoverableRegistryItem<K, V>
{
    public K Key;
    public V Value;

    public DiscoverableRegistryItem(K key, V value)
    {
        this.Key = key;
        this.Value = value;
    }
}

public abstract class DiscoverableRegistry<K, V> : AbstractRegistry<K, V>
{
    // Parse JSON object into registry item.
    public abstract DiscoverableRegistryItem<K, V> Parse(string rawJson);

    // Discover registry items from JSON files in a directory.
    public int Discover(string path)
    {
        string[] files = System.IO.Directory.GetFiles(path, "*.json");
        int discovered = 0;

        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            string content = System.IO.File.ReadAllText(file);
            DiscoverableRegistryItem<K, V> item = Parse(content);
            this.Set(item.Key, item.Value);
            discovered++;
        }

        return discovered;
    }
}
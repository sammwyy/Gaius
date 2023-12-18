using Newtonsoft.Json;

public class PropsRegistry : DiscoverableRegistry<string, TileMeta>
{
    public override DiscoverableRegistryItem<string, TileMeta> Parse(string rawJson)
    {
        TileMeta tileMeta = JsonConvert.DeserializeObject<TileMeta>(rawJson);
        return new(tileMeta.id, tileMeta);
    }

    // Static instance handling.
    private static PropsRegistry _instance = null;

    public static PropsRegistry GetInstance()
    {
        _instance ??= new PropsRegistry();
        return _instance;
    }
}

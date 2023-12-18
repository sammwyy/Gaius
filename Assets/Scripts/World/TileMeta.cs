using UnityEditor;
using UnityEngine;

public enum TileType
{
    Residential,
    Commercial,
    Industrial,
    Education,
    Fireman,
    Hospital,
    Police,
    Park,
    Religious,
    Road,
    WaterSource,
    WaterLine,
    WaterSupply,
    Invalid
}

public class TileMetaRate
{
    public int initial = -1;
    public int multiplier = -1;
}

public class TileMetaSize
{
    public int x = -1;
    public int z = -1;

    public bool ShouldChain()
    {
        return this.x > 1 || this.z > 1;
    }

    public bool IsXEven()
    {
        return this.x % 2 == 0;
    }

    public bool IsZEven()
    {
        return this.z % 2 == 0;
    }
}

public class TileMeta
{
    // Data.
    public string id = "<unknown>";
    public string displayName = "Foo";
    public string model;
    public PrefabAssetType prefab;
    public TileMetaSize size;

    // Common stats.
    public int limit = -1;
    public float cost = -1f;
    public float maxHealth = -1f;
    public float maxLevel = -1f;
    public TileType type = TileType.Invalid;

    // Residential properties.
    public int maxPopulation = -1;

    // Commercial properties.
    public int maxJobs = -1;

    // Industrial properties.
    public TileMetaRate resourceProduction;
    public TileMetaRate resourceConsumption;

    // Event properties.
    public bool isAffectedByDisaster = true;

    // Get prefab model.
    public GameObject GetPrefab()
    {
        return Resources.Load<GameObject>(this.model);
    }
}
using UnityEngine;

/**
    Tile is a single tile in the world.
*/
public class Tile : MonoBehaviour
{
    protected TileMeta _meta;
    protected TileState _state;

    // Start is called before the first frame update
    public void Start()
    {
        this._meta = null;
        this._state = ScriptableObject.CreateInstance<TileState>();
        this._state.status = TileStatus.Empty;
    }

    public void MetaSetTile(TileMeta meta)
    {
        this._state.Reset();
        this._state.status = TileStatus.Building;
        this._meta = meta;
    }

    public void MetaDestroy()
    {
        this._state.Reset();
        this._state.status = TileStatus.Destroyed;
    }

    public void MetaReset()
    {
        this._state.Reset();
        this._state.status = TileStatus.Empty;
    }

    public TileMeta Meta
    {
        get
        {
            return this._meta;
        }
    }

    // Only for debugging.
    public override string ToString()
    {
        Vector3 pos = transform.position;
        string child = this._meta == null ? "None" : this._meta.id;
        int level = this._state.level;
        float health = this._state.health;
        return $"x={pos.x}, z={pos.z}, child={child}, lv={level}, h={health}";
    }

    public static string AsString(Tile slot)
    {
        if (slot == null)
        {
            return "<none>";
        }
        else
        {
            return slot.ToString();
        }
    }
}
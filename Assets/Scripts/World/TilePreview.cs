using UnityEngine;

/**
    TilePreview is a preview of a tile.
*/
public class TilePreview : ScriptableObject
{
    public GameObject prefab;
    public TileMeta meta;

    // Set preview rendering. This is used when the tile is being previewed.
    public void SetPreviewRendering()
    {
        Renderer renderer = this.prefab.GetComponent<Renderer>();
        renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
        renderer.material.renderQueue = 3000;
    }

    // Set solid rendering. This is used when the tile is being placed.
    public void SetSolid()
    {
        Renderer renderer = this.prefab.GetComponent<Renderer>();
        renderer.material.color = new Color(1f, 1f, 1f, 1f);
        renderer.material.renderQueue = 2000;
    }

    // Set invalid rendering. This is used when the tile is being placed on an invalid slot.
    public void SetInvalid(bool invalid)
    {
        if (invalid)
        {
            this.prefab.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f, 0.5f);
        }
        else
        {
            this.prefab.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.5f);
        }
    }

    // Set preview position.
    public void MoveTo(Vector3 position)
    {
        this.prefab.transform.position = position;
    }

    // Set preview position based on a slot.
    public void MoveTo(TileWorldSlot slot)
    {
        Vector3 slotPos = slot.transform.position;
        TileMetaSize size = this.meta.size;

        float extraX = size.IsXEven() ? -0.5f : 0f;
        float extraY = 1f;
        float extraZ = size.IsZEven() ? -0.5f : 0f;

        Vector3 goPos = new(slotPos.x + extraX, slotPos.y + extraY, slotPos.z + extraZ);
        this.prefab.transform.position = goPos;
    }
}
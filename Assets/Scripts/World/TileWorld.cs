using System.Collections.Generic;
using UnityEngine;

/**
    TileWorld is the world where tiles are placed.
*/
public class TileWorld : MonoBehaviour
{
    public GameObject gridPrefab;
    public LayerMask gridLayerMask;

    // World settings.
    public int width;
    public int height;

    // Local state.
    TilePreview tilePreview;
    Dictionary<Vector2, GameObject> _grid;
    TileWorldSlot _hoveredSlot;
    TileWorldSlot _selectedSlot;
    GameObject _raycastHit;

    // Get centered selection of slots, based on size of tile.
    public TileWorldSlot[] GetCenteredSelection(TileWorldSlot centered, TileMetaSize size)
    {
        DebugCallstack.Push();
        List<TileWorldSlot> selection = new();
        Vector3 pos = centered.transform.position;

        int x = size.x;
        int z = size.z;

        int xHalf = x / 2;
        int zHalf = z / 2;

        int xStart = (int)pos.x - xHalf;
        int zStart = (int)pos.z - zHalf;

        for (int i = xStart; i < xStart + x; i++)
        {
            for (int j = zStart; j < zStart + z; j++)
            {
                Vector2 key = new(i, j);

                if (!this._grid.ContainsKey(key))
                {
                    return null;
                }

                selection.Add(this._grid[key].GetComponent<TileWorldSlot>());
            }
        }

        return selection.ToArray();
    }

    // Check if can place tile on desired slot, based on size of tile.
    public bool CanPlace(TileWorldSlot centered, TileMetaSize size)
    {
        DebugCallstack.Push();
        TileWorldSlot[] selection = GetCenteredSelection(centered, size);

        if (selection == null)
        {
            return false;
        }

        foreach (TileWorldSlot node in selection)
        {
            if (!node.CanPlace())
            {
                return false;
            }
        }

        return true;
    }

    // Get hovered slot.
    public TileWorldSlot GetHoveredSlot()
    {
        return this._hoveredSlot;
    }

    // Get selected slot.
    public TileWorldSlot GetSelectedSlot()
    {
        return this._selectedSlot;
    }

    // Get raycast hit.
    public GameObject GetRaycastHit()
    {
        return this._raycastHit;
    }

    // Set preview cursor.
    public void SetPreviewTile(TileMeta tileMeta)
    {
        DebugCallstack.Push();

        if (this.tilePreview != null)
        {
            Destroy(this.tilePreview.prefab);
        }

        GameObject previewGO = Instantiate(tileMeta.GetPrefab(), Vector3.zero, Quaternion.identity);
        this.tilePreview = ScriptableObject.CreateInstance<TilePreview>();
        this.tilePreview.prefab = previewGO;
        this.tilePreview.meta = tileMeta;
        this.tilePreview.SetPreviewRendering();

        if (this._hoveredSlot != null)
        {
            this._hoveredSlot.SetPreview(this.tilePreview);
        }
    }

    // Spawn all grid slots.
    public void Spawn()
    {
        DebugCallstack.Push();

        for (int x = 0; x < this.width; x++)
        {
            for (int z = 0; z < this.height; z++)
            {
                GameObject slot = Instantiate(this.gridPrefab, new Vector3(x, 0, z), Quaternion.identity);
                slot.transform.SetParent(this.transform);
                slot.name = string.Format("Slot ({0}, {1})", x, z);
                this._grid.Add(new Vector2(x, z), slot);

#if UNITY_EDITOR
                bool even = (x + z) % 2 == 0;

                if (even)
                {
                    slot.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
                else
                {
                    slot.GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 0.25f, 1f);
                }
#endif
            }
        }
    }

    // Handle grid slot selection.
    void HandleSlotSelection(TileWorldSlot slot)
    {
        if (this._selectedSlot != null)
        {
            this._selectedSlot.SetSelected(false);
        }

        slot.SetSelected(true);
        this._selectedSlot = slot;
    }

    // Handle hovered slot place in chain.
    void HandleSlotPlaceChain(TileWorldSlot slot, TileMeta meta)
    {
        TileWorldSlot[] selection = GetCenteredSelection(slot, meta.size);

        if (selection == null)
        {
            return;
        }

        foreach (TileWorldSlot node in selection)
        {
            if (!node.CanPlace())
            {
                return;
            }
        }

        if (slot.Place())
        {

            foreach (TileWorldSlot node in selection)
            {
                if (node != slot)
                {
                    node.SetChained(slot);
                    slot.AddChainNode(slot);
                }
            }

            this.tilePreview = null;
            this._hoveredSlot = null;
        }
    }

    // Handle hovered slot place.
    void HandleSlotPlace(TileWorldSlot slot)
    {
        if (slot.Place())
        {
            this.tilePreview = null;
            this._hoveredSlot = null;
        }
    }

    // Handle hover when cursor is over grid slot.
    void HandleSlotInteraction(TileWorldSlot slot)
    {
        // Handle hover.
        bool isNewHover = this._hoveredSlot != slot;

        if (this._hoveredSlot != null && isNewHover)
        {
            this._hoveredSlot.SetPreview(null);
            this._hoveredSlot.SetHover(false);
        }

        if (isNewHover)
        {
            DebugCallstack.Push();
            slot.SetHover(true);
            slot.SetPreview(this.tilePreview);
            this._hoveredSlot = slot;

            if (this.tilePreview != null)
            {
                this.tilePreview.SetInvalid(!this.CanPlace(slot, this.tilePreview.meta.size));
            }
        }

        // Handle selection.
        if (Input.GetMouseButtonDown(0))
        {
            if (this.tilePreview == null)
            {
                HandleSlotSelection(slot);
            }
            else
            {
                TileMeta meta = slot.GetTileMetaPreview();

                if (meta == null)
                {
                    return;
                }

                if (meta.size.ShouldChain())
                {
                    HandleSlotPlaceChain(slot, meta);
                }
                else
                {
                    HandleSlotPlace(slot);
                }
            }
        }
    }

    // Check if cursor is over game object with TileWorldSlot component.
    void HandleSlotInteraction(GameObject hitObject)
    {
        if (hitObject.TryGetComponent<TileWorldSlot>(out var slot))
        {
            HandleSlotInteraction(slot);
        }
    }

    // Check if cursor is over game object.
    void HandleSlotInteraction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100, this.gridLayerMask))
        {
            GameObject hitObject = hit.collider.gameObject;
            _raycastHit = hitObject;
            HandleSlotInteraction(hitObject);
        }
        else
        {
            _raycastHit = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleSlotInteraction();
    }

    // Start is called before the first frame update
    void Start()
    {
        this._grid = new();
        Spawn();
    }

    void Awake()
    {
        _instance = this;
    }

    // Static instance handling.
    private static TileWorld _instance;
    public static TileWorld GetInstance()
    {
        return _instance;
    }
}

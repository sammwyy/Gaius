using System.Collections.Generic;
using UnityEngine;

/**
    TileWorldSlot is a physical slot in the world, inheriting from Tile.
*/
public class TileWorldSlot : Tile
{
    private bool _hovered = false;
    private bool _selected = false;
    private TileWorldSlot _chainRoot;
    private List<TileWorldSlot> _chainNodes;
    private TilePreview _preview;
    private GameObject _child;
    private Color _originalColor;

    public TileMeta GetTileMetaPreview()
    {
        if (this._preview == null)
        {
            return null;
        }

        return this._preview.meta;
    }

    // Set hovered for this node only.
    public void SetHover(bool hovered)
    {
        this._hovered = hovered;
        UpdateDebug();
    }

    // Set hovered for the entire chain. If this node is not in a chain, it will be treated as a single node chain.
    public void SetChainHovered(bool hovered)
    {
        if (this._chainRoot == null)
        {
            this.SetHover(hovered);
            return;
        }

        this._chainRoot.SetHover(hovered);
        foreach (TileWorldSlot node in this._chainNodes)
        {
            node.SetHover(hovered);
        }
    }

    public void SetSelected(bool selected)
    {
        this._selected = selected;
        UpdateDebug();
    }

    public void SetChainSelected(bool selected)
    {
        if (this._chainRoot == null)
        {
            this.SetSelected(selected);
            return;
        }

        this._chainRoot.SetSelected(selected);
        foreach (TileWorldSlot node in this._chainNodes)
        {
            node.SetSelected(selected);
        }
    }

    public bool CanPlace()
    {
        return this._state.status == TileStatus.Empty && this._chainRoot == null;
    }

    public bool Place()
    {
        if (this._preview == null)
        {
            Debug.Log("No preview.");
            return false;
        }

        if (!this._hovered)
        {
            Debug.Log("Not hovered.");
            return false;
        }

        if (!CanPlace())
        {
            Debug.Log("Cannot place.");
            return false;
        }

        this._preview.SetSolid();
        this._preview.MoveTo(this);
        this.MetaSetTile(this._preview.meta);
        this._child = this._preview.prefab;
        this._hovered = false;
        this.UpdateDebug();
        return true;
    }

    public void SetPreview(TilePreview preview)
    {
        if (preview == null)
        {
            this._preview = null;
            return;
        }

        preview.MoveTo(this);
        this._preview = preview;
    }

    public void AddChainNode(TileWorldSlot node)
    {
        this._chainNodes.Add(node);
    }

    public void ClearChainNodes()
    {
        this._chainNodes.Clear();
    }

    public void SetChained(TileWorldSlot chained)
    {
        this._chainRoot = chained;
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _chainNodes = new List<TileWorldSlot>();
        _originalColor = this.GetComponent<Renderer>().material.color;
    }

    // Update debug information.
    void UpdateDebug()
    {
#if UNITY_EDITOR

        if (this._hovered)
        {
            this.GetComponent<Renderer>().material.color = Color.red;
        }
        else if (this._selected)
        {
            this.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            this.GetComponent<Renderer>().material.color = _originalColor;
        }
#endif
    }
}

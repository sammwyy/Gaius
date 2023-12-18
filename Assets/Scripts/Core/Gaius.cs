using UnityEngine;

public class Gaius : MonoBehaviour
{
    private PropsRegistry propsRegistry;

    void LoadResources()
    {
        string unityPath = Application.dataPath;

        // Load props.
        Debug.Log(unityPath + "/Data/Props");
        int props = this.propsRegistry.Discover(unityPath + "/Data/Props");
        Debug.Log($"Discovered {props} props.");
    }

    void Awake()
    {
        // Initialize registries.
        this.propsRegistry = PropsRegistry.GetInstance();

        // Load resources.
        this.LoadResources();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            TileWorld tileWorld = TileWorld.GetInstance();
            PropsRegistry propsRegistry = PropsRegistry.GetInstance();
            TileMeta tileMeta = propsRegistry.Get("HOUSE");
            tileWorld.SetPreviewTile(tileMeta);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            TileWorld tileWorld = TileWorld.GetInstance();
            PropsRegistry propsRegistry = PropsRegistry.GetInstance();
            TileMeta tileMeta = propsRegistry.Get("WATER_SOURCE");
            tileWorld.SetPreviewTile(tileMeta);
        }
    }

    // Static instance handling.
    private static Gaius _instance;

    public static Gaius GetInstance()
    {
        if (_instance == null)
        {
            _instance = GameObject.FindObjectOfType<Gaius>();
        }

        return _instance;
    }
}

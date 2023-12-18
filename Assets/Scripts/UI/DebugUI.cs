using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{
    // Local state.
    private bool _enabled = false;
    private TextMeshProUGUI _text;
    private string _template = "";
    private DebugProfiler _profiler;

    // Toggle debug UI.
    void Toggle()
    {
        this._enabled = !this._enabled;
        this._text.enabled = this._enabled;

        if (this._enabled)
        {
            this._profiler.Start();
        }
        else
        {
            this._profiler.Stop();
        }
    }

    // Update text.
    void UpdateDebugText()
    {
        if (this._profiler == null)
        {
            return;
        }

        this._text.text = this._profiler.Format(_template);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get TextMeshProUGUI component.
        this._text = this.GetComponentInChildren<TextMeshProUGUI>();

        // Get template.
        this._template = this._text.text;

        // Create debug state.
        this._profiler = ScriptableObject.CreateInstance<DebugProfiler>();
        this._profiler.Init();

        // Toggle debug UI by default if unity editor.
#if UNITY_EDITOR
        Toggle();
#else
        this._text.enabled = false;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Toggle();
        }

        if (this._enabled)
        {
            // Run every 200ms.
            if (Time.frameCount % 12 == 0)
            {
                this._profiler.UpdateData();
                UpdateDebugText();
            }
        }
    }
}

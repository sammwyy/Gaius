using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{
    // References.
    [SerializeField] private TextMeshProUGUI profiler;
    [SerializeField] private TextMeshProUGUI callstack;
    [SerializeField] private TextMeshProUGUI calllog;

    // Local state.
    private bool _enabled = false;
    private string _template = "";
    private DebugProfiler _profiler;
    private DebugCallstack _callstack;

    // Toggle debug UI.
    void Toggle()
    {
        this._enabled = !this._enabled;
        this.profiler.enabled = this._enabled;
        this.callstack.enabled = this._enabled;
        this.calllog.enabled = this._enabled;

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

        this.profiler.text = this._profiler.Format(_template);
    }

    void UpdateCallstackText()
    {
        if (this._callstack == null)
        {
            return;
        }

        this.callstack.text = $"Call stack:\n{this._callstack.GetCallstack()}";
    }

    void UpdateCalllogText()
    {
        if (this._callstack == null)
        {
            return;
        }

        this.calllog.text = $"Call log:\n{this._callstack.GetCalllog()}";
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get TextMeshProUGUI component.
        this.profiler = this.GetComponentInChildren<TextMeshProUGUI>();

        // Get template.
        this._template = this.profiler.text;

        // Create debug state.
        this._profiler = ScriptableObject.CreateInstance<DebugProfiler>();
        this._profiler.Init();

        // Create callstack.
        this._callstack = ScriptableObject.CreateInstance<DebugCallstack>();

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
                UpdateCallstackText();
                UpdateCalllogText();
            }
        }
    }
}

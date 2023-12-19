using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Gaius/DebugState")]
public class DebugProfiler : ScriptableObject
{
    // Profiler state.
    public CameraController cameraController;
    public TileWorld world;
    public bool enabled = false;

    // Profiler stats.
    public int minFPS = -1;
    public int maxFPS = -1;
    public int[] fpsHistory = new int[60];
    public int currentFPS = -1;

    public int minFrameTime = -1;
    public int maxFrameTime = -1;
    public int[] frameTimeHistory = new int[60];
    public int currentFrameTime = -1;

    public int memoryUsage = -1;
    public int memoryAllocated = -1;
    public int memoryReserved = -1;

    public string gpuBrand = "<none>";
    public int gpuMemoryAllocated = -1;

    public string cpuBrand = "<none>";
    public float cpuUsage = -1;
    public float cpuCoreCount = -1;

    public string scene = "<none>";

    // Local state.
    private Thread _cpuMeasureThread;

    void PushFPS(int fps)
    {
        this.currentFPS = fps;

        if (this.minFPS == -1 || fps < this.minFPS)
        {
            this.minFPS = fps;
        }

        if (this.maxFPS == -1 || fps > this.maxFPS)
        {
            this.maxFPS = fps;
        }

        for (int i = 0; i < this.fpsHistory.Length - 1; i++)
        {
            this.fpsHistory[i] = this.fpsHistory[i + 1];
        }

        this.fpsHistory[^1] = fps;
    }

    void PushFrameTime(int frameTime)
    {
        this.currentFrameTime = frameTime;

        if (this.minFrameTime == -1 || frameTime < this.minFrameTime)
        {
            this.minFrameTime = frameTime;
        }

        if (this.maxFrameTime == -1 || frameTime > this.maxFrameTime)
        {
            this.maxFrameTime = frameTime;
        }

        for (int i = 0; i < this.frameTimeHistory.Length - 1; i++)
        {
            this.frameTimeHistory[i] = this.frameTimeHistory[i + 1];
        }

        this.frameTimeHistory[^1] = frameTime;
    }

    int GetAVG(int[] history)
    {
        int sum = 0;
        int count = 0;

        foreach (int value in history)
        {
            if (value > 0)
            {
                sum += value;
                count++;
            }
        }

        return count > 0 ? sum / count : -1;
    }

    public string Format(string template)
    {
        GameObject raycastHit = this.world.GetRaycastHit();
        string raycastHitName = raycastHit == null ? "<none>" : raycastHit.name;

        return template
            .Replace("{minFPS}", this.minFPS.ToString())
            .Replace("{maxFPS}", this.maxFPS.ToString())
            .Replace("{avgFPS}", this.GetAVG(this.fpsHistory).ToString())
            .Replace("{fps}", this.currentFPS.ToString().PadRight(6, ' '))
            .Replace("{fpsColor}", this.currentFPS < 30 ? "red" : this.currentFPS < 60 ? "yellow" : "green")

            .Replace("{minFrameTime}", this.minFrameTime.ToString())
            .Replace("{maxFrameTime}", this.maxFrameTime.ToString())
            .Replace("{avgFrameTime}", this.GetAVG(this.frameTimeHistory).ToString())
            .Replace("{frameTime}", (this.currentFrameTime.ToString() + "ms").PadRight(6, ' '))
            .Replace("{frameTimeColor}", this.currentFrameTime > 32 ? "red" : this.currentFrameTime > 16 ? "yellow" : "green")

            .Replace("{memoryUsage}", this.memoryUsage.ToString())
            .Replace("{memoryAllocated}", this.memoryAllocated.ToString())
            .Replace("{memoryReserved}", this.memoryReserved.ToString())

            .Replace("{gpuBrand}", this.gpuBrand)
            .Replace("{gpuMemoryAllocated}", (this.gpuMemoryAllocated.ToString() + "MB").PadRight(6, ' '))

            .Replace("{cpuBrand}", this.cpuBrand)
            .Replace("{cpuUsage}", (this.cpuUsage.ToString("F1") + "%").PadRight(6, ' '))
            .Replace("{cpuCoreCount}", this.cpuCoreCount.ToString())


            .Replace("{gridSelected}", TileWorldSlot.AsString(this.world.GetSelectedSlot()))
            .Replace("{gridHovered}", TileWorldSlot.AsString(this.world.GetHoveredSlot()))

            .Replace("{camera}", this.cameraController.ToString())
            .Replace("{raycastHit}", raycastHitName)

            .Replace("{scene}", this.scene);
    }

    public void UpdateData()
    {
        // Samples counting.
        int currentFPS = (int)(1f / Time.unscaledDeltaTime);
        int currentFrameTime = (int)(Time.unscaledDeltaTime * 1000f);

        this.PushFPS(currentFPS);
        this.PushFrameTime(currentFrameTime);

        // RAM
        this.memoryUsage = (int)(Profiler.GetMonoHeapSizeLong() / 1024 / 1024);
        this.memoryAllocated = (int)(Profiler.GetTotalAllocatedMemoryLong() / 1024 / 1024);
        this.memoryReserved = (int)(Profiler.GetTotalReservedMemoryLong() / 1024 / 1024);

        // GPU
        this.gpuMemoryAllocated = (int)(Profiler.GetAllocatedMemoryForGraphicsDriver() / 1024 / 1024);

        // Misc data.
        this.scene = SceneManager.GetActiveScene().name;
    }


    void StartCPUMeasure()
    {
        var lastCpuTime = new TimeSpan(0);

        // This is ok since this is executed in a background thread
        while (true)
        {
            var cpuTime = new TimeSpan(0);

            // Get a list of all running processes in this PC
            var AllProcesses = Process.GetProcesses();

            // Sum up the total processor time of all running processes
            cpuTime = AllProcesses.Aggregate(cpuTime, (current, process) => current + process.TotalProcessorTime);

            // get the difference between the total sum of processor times
            // and the last time we called this
            var newCPUTime = cpuTime - lastCpuTime;

            // update the value of _lastCpuTime
            lastCpuTime = cpuTime;

            // The value we look for is the difference, so the processor time all processes together used
            // since the last time we called this divided by the time we waited
            // Then since the performance was optionally spread equally over all physical CPUs
            // we also divide by the physical CPU count
            cpuUsage = 100f * (float)newCPUTime.TotalSeconds / 3 / cpuCoreCount;

            // Wait for UpdateInterval
            Thread.Sleep(Mathf.RoundToInt(3 * 1000));
        }
    }

    public void Init()
    {
        // Get CPU data.
        this.cpuBrand = SystemInfo.processorType;
        this.cpuCoreCount = SystemInfo.processorCount;
        this._cpuMeasureThread = new Thread(StartCPUMeasure)
        {
            IsBackground = true,
            Priority = System.Threading.ThreadPriority.BelowNormal
        };

        // Get GPU data.
        this.gpuBrand = SystemInfo.graphicsDeviceName;
    }

    public void Start()
    {
        if (this.cameraController == null)
        {
            this.cameraController = FindObjectOfType<CameraController>();
        }

        if (this.world == null)
        {
            this.world = FindObjectOfType<TileWorld>();
        }

        this.UpdateData();
        this._cpuMeasureThread.Start();
    }

    public void Stop()
    {
        this._cpuMeasureThread.Abort();
    }
}
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

public class DebugCallstack : ScriptableObject
{
    private readonly List<string> _callstack;
    private readonly List<string> _calllog;

    private static readonly string COLOR_WORD = "#af3dff";
    private static readonly string COLOR_KEYS = "#55ffe1";
    private static readonly string COLOR_TYPE = "#3EB0EE";

    public DebugCallstack()
    {
        this._callstack = new List<string>();
        this._calllog = new List<string>();
        _instance = this;
    }

    public string GetCallstack()
    {
        string callstack = "";
        int index = 0;
        foreach (string call in this._callstack)
        {
            index++;
            callstack += $"{call}   <color=red>| {index}</color>\n";
        }

        return callstack;
    }

    public string GetCalllog()
    {
        string calllog = "";
        int index = 0;
        foreach (string call in this._calllog)
        {
            index++;
            calllog += $"{call}   <color=red>| {index}</color>\n";
        }

        return calllog;
    }

    public static void Clear()
    {
        if (_instance == null)
        {
            return;
        }

        _instance._callstack.Clear();
    }

    public static void PushStack(string message)
    {
        if (_instance == null)
        {
            return;
        }

        _instance._callstack.Add(message);
    }

    public static void PushLog(string message)
    {
        if (_instance == null)
        {
            return;
        }

        if (_instance._calllog.Count >= 20)
        {
            _instance._calllog.RemoveAt(0);
        }

        _instance._calllog.Add(message);
    }

    public static string Format(string functionName, ParameterInfo[] parameters)
    {
        string parametersString = "";
        foreach (ParameterInfo parameter in parameters)
        {
            parametersString += $"<color={COLOR_TYPE}>{parameter.ParameterType}</color>, ";
        }

        parametersString = parametersString.Length > 0 ? parametersString[..^2] : parametersString;
        return $"<color={COLOR_WORD}>{functionName}</color><color={COLOR_KEYS}>(</color>{parametersString}<color={COLOR_KEYS}>)</color>";
    }

    public static void Push()
    {
        Clear();

        StackTrace trace = new();
        StackFrame[] frames = trace.GetFrames();
        int index = -1;

        foreach (StackFrame frame in frames)
        {
            index++;

            if (index == 0)
            {
                continue;
            }

            MethodBase methodBase = frame.GetMethod();
            string descriptor = Format(methodBase.Name, methodBase.GetParameters());
            PushStack(descriptor);

            if (index == 1)
            {
                PushLog(descriptor);
            }
        }
    }

    private static DebugCallstack _instance;
}
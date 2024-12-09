using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TaskDebugger))]
public class TaskDebuggerEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TaskDebugger taskDebugger = (TaskDebugger)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Test Basic"))
        {
            taskDebugger.TestBasic();
        }

        if (GUILayout.Button("Test Sequential"))
        {
            taskDebugger.TestSequential();
        }

        if (GUILayout.Button("Test Async"))
        {
            taskDebugger.TestAsync();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor(typeof(GameAmbientLight))]
[CanEditMultipleObjects]
public class AmbientLightEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            return;
        }

        GameAmbientLight ambientLight = (GameAmbientLight)target;

        EditorGUILayout.Space();
        target.GameObject().GetComponent<Light>().intensity = ambientLight.baseIntensity;
    }
}

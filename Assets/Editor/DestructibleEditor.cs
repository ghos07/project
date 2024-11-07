// DestructibleEditor.cs
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Destructible))]
[CanEditMultipleObjects]
public class DestructibleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Destructible destructible = (Destructible)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Destroy"))
        {
            destructible.Invoke("ForceDestroy", 0);
        }

        if (GUILayout.Button("Repair"))
        {
            destructible.Repair();
        }
    }

    public void OnSceneGUI()
    {
        Destructible destructible = (Destructible)target;

        if (destructible.explodeOnDestroy)
        {
            Handles.color = Color.red;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            Handles.DrawWireDisc(destructible.explosionSource.position, Vector3.up, destructible.explosionRadius);
            Handles.DrawWireDisc(destructible.explosionSource.position, Vector3.forward, destructible.explosionRadius);
            Handles.DrawWireDisc(destructible.explosionSource.position, Vector3.right, destructible.explosionRadius);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.GreaterEqual;
            Handles.color = Color.red * 0.5f;
            Handles.DrawWireDisc(destructible.explosionSource.position, Vector3.up, destructible.explosionRadius);
            Handles.DrawWireDisc(destructible.explosionSource.position, Vector3.forward, destructible.explosionRadius);
            Handles.DrawWireDisc(destructible.explosionSource.position, Vector3.right, destructible.explosionRadius);
        }
    }
}

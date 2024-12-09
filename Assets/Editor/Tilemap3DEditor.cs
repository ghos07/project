using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tilemap3D))]
public class Tilemap3DEditor : Editor
{
    // This method is called when any value in the inspector is changed.
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Tilemap3D tilemap3D = (Tilemap3D)target;

        // If any serialized property is modified, rebuild the tilemap.
        if (GUI.changed)
        {
            tilemap3D.RebuildTilemap();
        }

        // If undo is performed, rebuild the tilemap.
        if (Event.current.commandName == "UndoRedoPerformed")
        {
            tilemap3D.RebuildTilemap();
        }
    }

    // If undo is performed, rebuild the tilemap.
    private void OnUndoRedo()
    {
        Tilemap3D tilemap3D = (Tilemap3D)target;
        tilemap3D.RebuildTilemap();
    }
}

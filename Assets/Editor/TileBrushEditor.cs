using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using Unity.VisualScripting;

public class TileBrushEditor : EditorWindow
{
    [SerializeField]
    private Tilemap3D tilemap3D;

    private bool isDrawing = false;
    private bool lockYDraw = false;
    private bool draw1Below = false;
    private bool fillBelow = false;
    private bool fillByResize = false;
    private float fillToY = 0;

    private Vector3Int lastTilePos;
    private TileData currentTileData = new TileData(0);
    private TileData fillTileData = new TileData(0);
    private Vector3Int gridSize = new Vector3Int(10, 10, 1);

    private ToolType selectedTool = ToolType.Draw;
    private float drawingPlane = 0;

    private Vector3 mouseNormal = Vector3.up;

    private enum ToolType
    {
        Select,
        Draw,
        Delete,
    }

    [MenuItem("Tools/Tile Brush Editor")]
    private static void ShowWindow()
    {
        GetWindow<TileBrushEditor>("Tile Brush Editor").Show();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label("Tile Brush Editor", EditorStyles.boldLabel);

        tilemap3D = (Tilemap3D)EditorGUILayout.ObjectField("Tilemap 3D", tilemap3D, typeof(Tilemap3D), true);
        selectedTool = (ToolType)EditorGUILayout.EnumPopup("Tool", selectedTool);

        if (selectedTool == ToolType.Draw)
        {
            currentTileData.tileId = EditorGUILayout.IntField("Tile ID", currentTileData.tileId);
            lockYDraw = EditorGUILayout.Toggle("Lock Y", lockYDraw);
            draw1Below = EditorGUILayout.Toggle("Draw 1 Below", draw1Below);
            fillBelow = EditorGUILayout.Toggle("Fill Below", fillBelow);
            if (fillBelow)
            {
                fillTileData.tileId = EditorGUILayout.IntField("Fill Tile ID", fillTileData.tileId);
                fillToY = tilemap3D.TilespaceToWorldspace(tilemap3D.WorldspaceToTilespace(new(0, EditorGUILayout.FloatField("Fill To Y", fillToY), 0))).y;
                fillByResize = EditorGUILayout.Toggle("Fill By Resize", fillByResize);
            }

            // Preview prefab
            if (currentTileData.tileId >= 0 && currentTileData.tileId < tilemap3D.tilePrefabs.Count)
            {
                GUILayout.Label("Preview");
                GUILayout.Box(AssetPreview.GetAssetPreview(tilemap3D.tilePrefabs[currentTileData.tileId]), GUILayout.Width(64), GUILayout.Height(64));
            }
        }

        if (GUILayout.Button("Clear Tiles") && tilemap3D != null)
        {
            tilemap3D.ClearTilemap();
            tilemap3D.RebuildTilemap();
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (tilemap3D == null || selectedTool == ToolType.Select) return;

        Event e = Event.current;
        if (e == null || Camera.current == null) return;

        Vector3 cursorWorldPosition = GetCursorWorldPosition(e);
        if (cursorWorldPosition == Vector3.zero) return;

        DrawCursorIndicator(cursorWorldPosition);

        Vector3Int tilePos = tilemap3D.WorldspaceToTilespace(cursorWorldPosition);

        // Calculate sides of hexagon in this method and draw them
        // Draw hexagon
        Vector3[] hexagon = new Vector3[6]; 
        for (int i = 0; i < 6; i++)
        {
            float angle = 60 * i;
            hexagon[i] = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
        }
        for (int i = 0; i < 6; i++)
        {
            Handles.DrawLine(tilemap3D.TilespaceToWorldspace(tilePos), tilemap3D.TilespaceToWorldspace(tilePos) + hexagon[i]);
        }

        HandleInput(e, tilePos);

        if (isDrawing)
        {
            Vector3Int lastTilePosOrg = tilePos;
            if (IsTileChangeSignificant(tilePos, lastTilePos))
            {
                if (draw1Below)
                {
                    tilePos.z--;
                }
                if (selectedTool == ToolType.Draw)
                {
                    DrawTileAtPosition(tilePos);
                    tilemap3D.RebuildTilemap();
                }
                if (fillBelow)
                {
                    tilePos.z--;
                    if (fillByResize)
                    {
                        // Set targetPos to point between tilePos and fillToY
                        float middleY = (tilemap3D.TilespaceToWorldspace(tilePos).y + fillToY) / 2;
                        Vector3 targetTileWorldSpace = tilemap3D.TilespaceToWorldspace(tilePos);
                        Vector3Int middlePos = tilemap3D.WorldspaceToTilespace(new Vector3(targetTileWorldSpace.x, middleY, targetTileWorldSpace.z));
                        float offsetY = middleY - tilemap3D.TilespaceToWorldspace(middlePos).y;

                        TileData fillTile = fillTileData.Clone();
                        fillTile.SetScale(new(1, Mathf.Abs(targetTileWorldSpace.y - fillToY) / tilemap3D.tileSpacing.y, 1));
                        fillTile.SetOffset(new(0, offsetY, 0));

                        for (int y = tilePos.z - 1; y >= tilemap3D.WorldspaceToTilespace(new Vector3(0, fillToY, 0)).z; y--)
                        {
                            tilePos.z = y;
                            tilemap3D.tilemap.RemoveTile(tilePos.x, tilePos.y, tilePos.z);
                        }

                        DrawTileAtPosition(middlePos, fillTile);
                        tilemap3D.RebuildTilemap();
                    }
                    else
                    {
                        for (int y = tilePos.z; y >= tilemap3D.WorldspaceToTilespace(new Vector3(0, fillToY, 0)).z; y--)
                        {
                            tilePos.z = y;
                            DrawTileAtPosition(tilePos, fillTileData);
                        }
                        tilemap3D.RebuildTilemap();
                    }
                }

                lastTilePos = lastTilePosOrg;
            }
            drawingPlane = cursorWorldPosition.y;
        }

        SceneView.RepaintAll();
    }

    private Vector3 GetCursorWorldPosition(Event e)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (isDrawing && lockYDraw)
        {
            Plane lockedPlane = new Plane(Vector3.up, new Vector3(0, drawingPlane, 0));
            return lockedPlane.Raycast(ray, out float dist) ? ray.GetPoint(dist) : Vector3.zero;
        }

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Return point and set normal for drawing
            mouseNormal = hit.normal;
            return hit.point;
        }

        mouseNormal = SceneView.lastActiveSceneView.camera.transform.position.y > 0 ? Vector3.up: Vector3.down;

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        return groundPlane.Raycast(ray, out float distance) ? ray.GetPoint(distance) : Vector3.zero;
    }

    private void DrawCursorIndicator(Vector3 position)
    {
        Handles.color = Color.red;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        Handles.DrawSolidDisc(position, Vector3.up, 0.1f);

        Handles.color = new Color(1, 0, 0, 0.2f);
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
        Handles.DrawSolidDisc(position, Vector3.up, 0.1f);
    }

    private void HandleInput(Event e, Vector3Int tilePos)
    {
        if (e.type == EventType.MouseDown && e.button == 0 && selectedTool == ToolType.Draw)
        {
            isDrawing = true;
            lastTilePos = Vector3Int.one * int.MaxValue;
            e.Use();
        }
        else if (e.type == EventType.MouseUp && e.button == 0)
        {
            isDrawing = false;
            drawingPlane = 0;
            e.Use();
        }
    }

    private bool IsTileChangeSignificant(Vector3Int current, Vector3Int last)
    {
        return Mathf.Abs(current.z - last.z) > 2 || current.x != last.x || current.y != last.y;
    }

    private void DrawTileAtPosition(Vector3Int tilePos)
    {
        tilemap3D?.tilemap.SetTile(tilePos.x, tilePos.y, tilePos.z, currentTileData, true);
    }

    private void DrawTileAtPosition(Vector3Int tilePos, TileData tileData)
    {
        tilemap3D?.tilemap.SetTile(tilePos.x, tilePos.y, tilePos.z, tileData, true);
    }
}

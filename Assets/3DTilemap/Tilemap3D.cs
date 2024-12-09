using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;



/// <summary>
/// Creates a 3D tilemap using stacked 2D tilemaps.
/// </summary>
public class Tilemap3D : MonoBehaviour
{
    [SerializeField] public SavedTilemap tilemap;
    [SerializeField] public List<GameObject> tilePrefabs;
    [SerializeField] public Vector3 tileScale = Vector3.one;
    [SerializeField] public Vector3 tileSpacing = Vector3.one;

    private Dictionary<Vector3Int, GameObject> instantiatedTiles = new Dictionary<Vector3Int, GameObject>();
    private List<GameObject> foundTiles = new List<GameObject>();

    /// <summary>
    /// Converts tile space coordinates to world space coordinates.
    /// Supports rectangular and hexagonal grid layouts.
    /// </summary>
    public Vector3 TilespaceToWorldspace(Vector3Int tilePos)
    {
        if (tilemap.gridShape == GridShape3D.Rect)
        {
            return new Vector3(
                tilePos.x * tileSpacing.x,
                tilePos.z * tileSpacing.y,
                tilePos.y * tileSpacing.z
            );
        }

        // Hexagonal grid (flat-topped hex)
        float worldX = tileSpacing.x * (tilePos.x + tilePos.y * 0.5f);
        float worldZ = tileSpacing.z * (tilePos.y * Mathf.Sqrt(3) / 2.0f);
        return new Vector3(worldX, tilePos.z * tileSpacing.y, worldZ);
    }

    /// <summary>
    /// Converts world space coordinates to tile space coordinates.
    /// </summary>
    public Vector3Int WorldspaceToTilespace(Vector3 worldPos)
    {
        worldPos.z *= 2;

        if (tilemap.gridShape == GridShape3D.Rect)
        {
            return new Vector3Int(
                Mathf.RoundToInt(worldPos.x / tileSpacing.x),
                Mathf.RoundToInt(worldPos.z / tileSpacing.z),
                Mathf.RoundToInt(worldPos.y / tileSpacing.y)
            );
        }

        // Hexagonal grid (flat-topped hex)
        float q = (worldPos.x / tileSpacing.x) - (worldPos.z / (tileSpacing.z * Mathf.Sqrt(3)) * 0.5f);
        float r = (worldPos.z / (tileSpacing.z * Mathf.Sqrt(3)));
        Vector2Int hexCoords = RoundHexCoordinates(new Vector2(q, r));
        int z = Mathf.RoundToInt(worldPos.y / tileSpacing.y);
        return new Vector3Int(hexCoords.x, hexCoords.y, z);
    }

    /// <summary>
    /// Rounds fractional hex coordinates to the nearest integer.
    /// </summary>
    private Vector2Int RoundHexCoordinates(Vector2 fractionalHex)
    {
        float q = fractionalHex.x, r = fractionalHex.y, s = -(q + r);
        int roundedQ = Mathf.RoundToInt(q);
        int roundedR = Mathf.RoundToInt(r);
        int roundedS = Mathf.RoundToInt(s);

        float qDiff = Mathf.Abs(roundedQ - q);
        float rDiff = Mathf.Abs(roundedR - r);
        float sDiff = Mathf.Abs(roundedS - s);

        if (qDiff > rDiff && qDiff > sDiff)
            roundedQ = -(roundedR + roundedS);
        else if (rDiff > sDiff)
            roundedR = -(roundedQ + roundedS);

        return new Vector2Int(roundedQ, roundedR);
    }

    /// <summary>
    /// Builds or updates the tilemap at a specific position.
    /// </summary>
    private void BuildTilemapAtPosition(Vector3Int tilePos)
    {
        TileData? tileData = tilemap.GetTile(tilePos.x, tilePos.y, tilePos.z);
        if (tileData == null || tileData.Value.tileId < 0 || tileData.Value.tileId >= tilePrefabs.Count)
            return;

        if (!instantiatedTiles.ContainsKey(tilePos))
        {
            Vector3 worldPos = TilespaceToWorldspace(tilePos);
            GameObject tileInstance = Instantiate(tilePrefabs[tileData.Value.tileId], worldPos + tileData.Value.GetOffset(), tileData.Value.GetRotation(), transform);
            tileInstance.transform.localScale = Vector3.Scale(tileInstance.transform.localScale, tileData.Value.GetScale());
            tileInstance.name = $"Tile ({tilePos.x}, {tilePos.y}, {tilePos.z}) - ID {tileData.Value.tileId}";
            instantiatedTiles[tilePos] = tileInstance;

            TagManager.GetTagManager(tileInstance).AddTag(Tag.TilemanagerTile);
        }
    }

    /// <summary>
    /// Destroys all instantiated tiles.
    /// </summary>
    public void ClearInstances()
    {
        foreach (var tile in instantiatedTiles.Values)
        {
            if (Application.isPlaying)
                Destroy(tile);
            else
                DestroyImmediate(tile);
        }
        instantiatedTiles.Clear();

        foreach (var tile in foundTiles)
        {
            if (Application.isPlaying)
                Destroy(tile);
            else
                DestroyImmediate(tile);
        }
        foundTiles.Clear();
    }

    /// <summary>
    /// Clears the entire tilemap and its instances.
    /// </summary>
    public void ClearTilemap()
    {
        tilemap.Clear();
        ClearInstances();
    }

    /// <summary>
    /// Rebuilds the entire tilemap from saved data.
    /// </summary>
    [ContextMenu("Rebuild Tilemap")]
    public void RebuildTilemap()
    {
        ClearInstances();
        tilemap.ForEachTile((_, z, tilePos, tileData) => BuildTilemapAtPosition(new Vector3Int(tilePos.x, tilePos.y, z)));
    }

    [ContextMenu("Identify Placed Tiles")]
    public void FindTiles()
    {
        // Loop through all children of the tilemap and check for TilemanagerTile tag
        foreach (Transform child in transform)
        {
            if (TagManager.GetTagManager(child.gameObject).HasTag(Tag.TilemanagerTile))
            {
                foundTiles.Add(child.gameObject);
            }
        }
    }

    /// <summary>
    /// Example setup for testing.
    /// </summary>
    private void Start()
    {
        // Test pattern: Circle in layer 0.
        for (int x = -5; x <= 5; x++)
        {
            for (int y = -5; y <= 5; y++)
            {
                if (x * x + y * y <= 25)
                    tilemap.SetTile(x, y, 0, new TileData(0), true);
            }
        }

        ClearInstances();
        RebuildTilemap();
    }
}

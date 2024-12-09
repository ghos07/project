using System.Collections.Generic;
using System;

using UnityEngine;

/// <summary>
/// Represents data for a single tile.
/// </summary>
public struct TileData
{
    /// <summary>
    /// ID / Index of the tile prefab to instantiate.
    /// </summary>
    public int tileId;

    /// <summary>
    /// Array of floats representing the position(offset), rotation, and scale of the tile.
    /// [0-2] = position, [3-6] = rotation, [7-9] = scale.
    /// </summary>
    public float[] transforms;

    /// <summary>
    /// Dictionary of integers saved to the tilemap.
    /// </summary>
    public Dictionary<string, int> savedInts;

    /// <summary>
    /// Dictionary of floats saved to the tilemap.
    /// </summary>
    public Dictionary<string, float> savedFloats;

    /// <summary>
    /// Sets an entry in the saved integer dictionary.
    /// </summary>
    /// <param name="key"> The key to store the value under. </param>
    /// <param name="value"> The value to store. </param>
    public readonly void SetInt(string key, int value)
    {
        savedInts[key] = value;
    }

    /// <summary>
    /// Retrieves an integer from the saved dictionary.
    /// </summary>
    /// <param name="key"> The key to retrieve the value from. </param>
    /// <returns> The value stored under the key, or null if not found. </returns>
    public readonly int? GetInt(string key)
    {
        return savedInts.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// Sets an entry in the saved float dictionary.
    /// </summary>
    /// <param name="key"> The key to store the value under. </param>
    /// <param name="value"> The value to store. </param>
    public readonly void SetFloat(string key, float value)
    {
        savedFloats[key] = value;
    }

    /// <summary>
    /// Retrieves a float from the saved dictionary.
    /// </summary>
    /// <param name="key"> The key to retrieve the value from. </param>
    /// <returns> The value stored under the key, or null if not found. </returns>
    public readonly float? GetFloat(string key)
    {
        return savedFloats.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// Copies the position, rotation, and scale from a transform to the tile data.
    /// </summary>
    /// <param name="transform"> The transform to copy from. </param>
    public readonly void SetTransform(Transform transform)
    {
        transforms[0] = transform.position.x;
        transforms[1] = transform.position.y;
        transforms[2] = transform.position.z;
        transforms[3] = transform.rotation.x;
        transforms[4] = transform.rotation.y;
        transforms[5] = transform.rotation.z;
        transforms[6] = transform.rotation.w;
        transforms[7] = transform.localScale.x;
        transforms[8] = transform.localScale.y;
        transforms[9] = transform.localScale.z;
    }

    /// <summary>
    /// Sets the position/offset of the tile.
    /// </summary>
    /// <param name="offset"> The offset to set. </param>
    public readonly void SetOffset(Vector3 offset)
    {
        transforms[0] = offset.x;
        transforms[1] = offset.y;
        transforms[2] = offset.z;
    }

    /// <summary>
    /// Sets the rotation of the tile.
    /// </summary>
    /// <param name="rotation"> The rotation to set. </param>
    public readonly void SetRotation(Quaternion rotation)
    {
        transforms[3] = rotation.x;
        transforms[4] = rotation.y;
        transforms[5] = rotation.z;
        transforms[6] = rotation.w;
    }

    /// <summary>
    /// Sets the scale of the tile.
    /// </summary>
    /// <param name="scale"> The scale to set. </param>
    public readonly void SetScale(Vector3 scale)
    {
        transforms[7] = scale.x;
        transforms[8] = scale.y;
        transforms[9] = scale.z;
    }

    /// <summary>
    /// Retrieves the position/offset of the tile.
    /// </summary>
    /// <returns></returns>
    public readonly Vector3 GetOffset() => new Vector3(transforms[0], transforms[1], transforms[2]);

    /// <summary>
    /// Retrieves the rotation of the tile.
    /// </summary>
    /// <returns></returns>
    public readonly Quaternion GetRotation() => new Quaternion(transforms[3], transforms[4], transforms[5], transforms[6]);

    /// <summary>
    /// Retrieves the scale of the tile.
    /// </summary>
    /// <returns></returns>
    public readonly Vector3 GetScale() => new Vector3(transforms[7], transforms[8], transforms[9]);

    /// <summary>
    /// Constructs a new tile data object with the specified ID.
    /// </summary>
    /// <param name="id"> The ID / Index of the tile prefab to instantiate. </param>
    public TileData(int id)
    {
        tileId = id;
        savedInts = new Dictionary<string, int>();
        savedFloats = new Dictionary<string, float>();
        transforms = new float[10];
        SetScale(Vector3.one);
    }

    /// <summary>
    /// Clones the tile data object.
    /// </summary>
    /// <returns> A new tile data object with the same values. </returns>
    public TileData Clone()
    {
        TileData clone = new TileData(tileId);
        clone.transforms = (float[])transforms.Clone();
        clone.savedInts = new Dictionary<string, int>(savedInts);
        clone.savedFloats = new Dictionary<string, float>(savedFloats);
        return clone;
    }
}

/// <summary>
/// Represents a single layer of tiles in the 3D tilemap.
/// </summary>
public struct TileLayer
{
    public Dictionary<Vector2Int, TileData> tiles;

    /// <summary>
    /// Initializes the tiles dictionary.
    /// </summary>
    public void InitializeLayer()
    {
        tiles = new Dictionary<Vector2Int, TileData>();
    }

    /// <summary>
    /// Retrieves a tile from the layer.
    /// </summary>
    /// <param name="x"> The x-coordinate of the tile. </param>
    /// <param name="y"> The y-coordinate of the tile. </param>
    /// <returns> The tile data at the specified coordinates, or null if not found. </returns>
    public TileData? GetTile(int x, int y)
    {
        return tiles.TryGetValue(new Vector2Int(x, y), out var tile) ? tile : (TileData?)null;
    }

    /// <summary>
    /// Sets a tile in the layer.
    /// </summary>
    /// <param name="pos"> The position of the tile. </param>
    /// <returns> The tile data to set at the specified coordinates. </returns>
    public TileData? GetTile(Vector2Int pos) => GetTile(pos.x, pos.y);

    /// <summary>
    /// Sets a tile in the layer.
    /// </summary>
    /// <param name="x"> The x-coordinate of the tile. </param>
    /// <param name="y"> The y-coordinate of the tile. </param>
    /// <param name="tile"> The tile data to set at the specified coordinates. </param>
    public void SetTile(int x, int y, TileData tile)
    {
        tiles[new Vector2Int(x, y)] = tile;
    }

    /// <summary>
    /// Sets a tile in the layer.
    /// </summary>
    /// <param name="pos"> The position of the tile. </param>
    /// <param name="tile"> The tile data to set at the specified coordinates. </param>
    public void SetTile(Vector2Int pos, TileData tile) => SetTile(pos.x, pos.y, tile);
}

/// <summary>
/// A saved representation of a 3D tilemap. It contains multiple layers and configuration details.
/// </summary>
[CreateAssetMenu(fileName = "NewSavedTilemap", menuName = "Tilemap/3D Tilemap")]
public class SavedTilemap : ScriptableObject
{
    public GridShape3D gridShape;
    public Dictionary<int, TileLayer> layers;

    public SavedTilemap()
    {
        layers = new Dictionary<int, TileLayer>();
    }

    public TileData? GetTile(int x, int y, int z, bool ensureExists)
    {
        if (ensureExists)
        {
            EnsureLayerExists(z);
            return layers[z].GetTile(x, y);
        }
        else if (layers.ContainsKey(z))
        {
            return layers[z].GetTile(x, y);
        }
        else
        {
            return null;
        }
    }

    public TileData? GetTile(int x, int y, int z) => GetTile(x, y, z, false);

    public void SetTile(int x, int y, int z, TileData tile, bool ensureExists)
    {
        if (ensureExists)
        {
            EnsureLayerExists(z);
            layers[z].SetTile(x, y, tile);
        }
        else
        {
            if (layers.ContainsKey(z))
            {
                layers[z].SetTile(x, y, tile);
            }
            else
            {
                Debug.LogWarning($"Layer {z} does not exist.");
            }
        }
    }

    public void SetTile(int x, int y, int z, TileData tile) => SetTile(x, y, z, tile, false);

    public void RemoveTile(int x, int y, int z)
    {
        if (layers.ContainsKey(z))
        {
            layers[z].tiles.Remove(new Vector2Int(x, y));
        }
    }

    public void RemoveLayer(int z)
    {
        layers.Remove(z);
    }

    private void EnsureLayerExists(int z)
    {
        if (!layers.ContainsKey(z))
        {
            // Add new layer and initialize it
            TileLayer newLayer = new TileLayer();
            newLayer.InitializeLayer();
            layers[z] = newLayer;
        }
    }

    /// <summary>
    /// Iterates through all layers and tiles, performing an action on each tile.
    /// Do not modify the tilemap directly; use the clone provided in the action.
    /// </summary>
    /// <param name="action">The action to perform, with parameters for 
    public void ForEachTile(Action<SavedTilemap, int, Vector2Int, TileData> action)
    {
        SavedTilemap clone = Clone();

        foreach (var layerEntry in layers)
        {
            int layerIndex = layerEntry.Key;
            TileLayer tileLayer = layerEntry.Value;

            foreach (var tileEntry in tileLayer.tiles)
            {
                Vector2Int tilePos = tileEntry.Key;
                TileData tileData = tileEntry.Value;

                action(clone, layerIndex, tilePos, tileData);
            }
        }

        SyncChangesFromClone(clone);
    }

    /// <summary>
    /// Iterates through each layer in the tilemap.
    /// Do not modify the tilemap directly; use the clone provided in the action.
    /// </summary>
    /// <param name="action">The action to perform, with parameters for
    public void ForEachLayer(Action<SavedTilemap, int, TileLayer> action)
    {
        SavedTilemap clone = Clone();

        foreach (var layerEntry in layers)
        {
            int layerIndex = layerEntry.Key;
            TileLayer tileLayer = layerEntry.Value;

            action(clone, layerIndex, tileLayer);
        }

        SyncChangesFromClone(clone);
    }

    /// <summary>
    /// Iterates through all tiles in a specified layer.
    /// Do not modify the tilemap directly; use the clone provided in the action.
    /// </summary>
    /// <param name="layerIndex">The index of the layer to iterate through.</param>
    /// <param name="action">The action to perform, with parameters for 
    public void ForEachTileInLayer(int layerIndex, Action<SavedTilemap, int, Vector2Int, TileData> action)
    {

        SavedTilemap clone = Clone();

        if (layers.TryGetValue(layerIndex, out TileLayer tileLayer))
        {
            foreach (var tileEntry in tileLayer.tiles)
            {
                Vector2Int tilePos = tileEntry.Key;
                TileData tileData = tileEntry.Value;

                action(clone, layerIndex, tilePos, tileData);
            }
        }
        else
        {
            Debug.LogWarning($"Layer {layerIndex} does not exist.");
        }

        SyncChangesFromClone(clone);
    }

    /// <summary>
    /// Clear all tiles in the tilemap.
    /// </summary>
    public void Clear()
    {
        layers.Clear();
    }

    /// <summary>
    /// Creates a deep clone of the SavedTilemap, including all layers and tiles.
    /// </summary>
    public SavedTilemap Clone()
    {
        SavedTilemap clone = CreateInstance<SavedTilemap>();
        clone.gridShape = this.gridShape;

        foreach (var layerEntry in this.layers)
        {
            int layerIndex = layerEntry.Key;
            TileLayer originalLayer = layerEntry.Value;

            TileLayer clonedLayer = new TileLayer();
            clonedLayer.tiles = new Dictionary<Vector2Int, TileData>(originalLayer.tiles);

            clone.layers[layerIndex] = clonedLayer;
        }

        return clone;
    }

    /// <summary>
    /// Synchronizes changes from a cloned SavedTilemap back to the original.
    /// </summary>
    /// <param name="clone">The cloned tilemap with changes to apply.</param>
    private void SyncChangesFromClone(SavedTilemap clone)
    {
        this.gridShape = clone.gridShape;
        this.layers = clone.layers;
    }


}
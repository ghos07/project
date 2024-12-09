using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTileCreated : MonoBehaviour
{
    /// <summary>
    /// Called when a tile is created.
    /// Changes to the tile data will not be reflected in the tilemap.
    /// Use Tilemap3D.SetTileData to save any changes.
    /// </summary>
    /// <param name="tilemap"> The tilemap that the tile was created on. </param>
    /// <param name="tileData"> The tile data of the created tile. </param>
    public UnityEvent<Tilemap3D, TileData> onTileCreated = new UnityEvent<Tilemap3D, TileData>();
}

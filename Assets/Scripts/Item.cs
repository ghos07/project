using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string id;
    public List<Tag> tags = new();
    public GameObject prefab;
    public int value;
}
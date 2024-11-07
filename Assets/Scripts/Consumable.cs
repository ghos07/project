using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
public class Consumable : Item
{
    public int hungerValue;
    public int healthValue;

    public event System.Action OnConsumed;

    private void OnValidate()
    {
        if (itemName == "" && id == "" && tags.Count == 0 && prefab == null && hungerValue == 0 && healthValue == 0)
        {
            tags.Add(Tag.ItemCanBeEaten);
            tags.Add(Tag.ItemConsumable);
        }
    }
}

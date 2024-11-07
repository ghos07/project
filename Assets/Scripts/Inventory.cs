using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemStack> items = new();
    [SerializeField] private int capacity = 20;
    [SerializeField] private bool stackItems = true;
    [SerializeField] private int maxStackSize = 5;

    [SerializeField] private List<Tag> acceptedItemTags = new();

    public bool isFull()
    {
        return items.Count >= capacity;
    }

    public bool AddItem(Item item, int amount = 1)
    {
        if (isFull())
        {
            return false;
        }

        if (stackItems)
        {
            foreach (ItemStack stack in items)
            {
                if (stack.item == item)
                {
                    if (stack.stackSize + amount <= maxStackSize)
                    {
                        stack.stackSize += amount;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        if (amount > maxStackSize)
        {
            return false;
        }

        items.Add(new ItemStack { item = item, stackSize = amount });
        return true;
    }
    
    public bool RemoveItem(Item item, int amount = 1)
    {
        foreach (ItemStack stack in items)
        {
            if (stack.item == item)
            {
                if (stack.stackSize >= amount)
                {
                    stack.stackSize -= amount;
                    if (stack.stackSize == 0)
                    {
                        items.Remove(stack);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

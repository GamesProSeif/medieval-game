using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem 
{
    public string itemName;
    public int count;
    public GameObject prefab;
    public bool stackable;

    public InventoryItem() { }

    public InventoryItem(string itemName, int count, GameObject prefab, bool stackable)
    {
        this.itemName = itemName;
        this.count = count;
        this.prefab = prefab;
        this.stackable = stackable;
    }
}

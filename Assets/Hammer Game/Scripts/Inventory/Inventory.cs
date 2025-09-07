using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    [SerializeField] private List<InventoryItem> _items = new List<InventoryItem>();

    public void CropHarvestedCallback(CropType cropType)
    {
        bool cropFound = false;

        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].ItemType == ItemType.Crop && _items[i].CropType == cropType)
            {
                _items[i].Amount++;
                cropFound = true;
                break;
            }
        }

        if (cropFound)
            return;

        _items.Add(new InventoryItem(cropType, 1));
    }

    public void ResourceCollectedCallback(ResourceType resourceType)
    {
        bool resourceFound = false;

        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].ItemType == ItemType.Resource && _items[i].ResourceType == resourceType)
            {
                _items[i].Amount++;
                resourceFound = true;
                break;
            }
        }

        if (resourceFound)
            return;

        _items.Add(new InventoryItem(resourceType, 1));
    }

    public InventoryItem[] GetInventoryItems()
    {
        return _items.ToArray();
    }

    public void Clear()
    {
        _items.Clear();
    }

    public void DebugInventory()
    {
        foreach(InventoryItem item in _items)
        {
            if (item.ItemType == ItemType.Crop)
                Debug.Log(item.Amount + " " + item.CropType + " in inventory.");
            else if (item.ItemType == ItemType.Resource)
                Debug.Log(item.Amount + " " + item.ResourceType + " in inventory.");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public ItemType ItemType;
    public CropType CropType;
    public ResourceType ResourceType;
    public int Amount;

    public InventoryItem(CropType cropType, int amount)
    {
        ItemType = ItemType.Crop;
        CropType = cropType;
        Amount = amount;
    }

    public InventoryItem(ResourceType resourceType, int amount)
    {
        ItemType = ItemType.Resource;
        ResourceType = resourceType;
        Amount = amount;
    }
}
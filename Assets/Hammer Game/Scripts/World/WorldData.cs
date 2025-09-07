using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnlockableData
{
    public string objectName;
    public int price;

    public UnlockableData(string name, int price)
    {
        this.objectName = name;
        this.price = price;
    }
}

public class WorldData
{
    public List<int> ChunkPrices;
    public List<UnlockableData> UnlockablePrices;

    public WorldData()
    {
        ChunkPrices = new List<int>();
        UnlockablePrices = new List<UnlockableData>();
    }
}

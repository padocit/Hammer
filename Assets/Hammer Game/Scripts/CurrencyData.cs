using System;
using UnityEngine;

[Serializable]
public class CurrencyData
{
    public CurrencyType type;
    public int amount;
    public string displayName;
    public string saveKey;
    public string uiTag;

    public CurrencyData(CurrencyType type, string displayName, string saveKey, string uiTag)
    {
        this.type = type;
        this.displayName = displayName;
        this.saveKey = saveKey;
        this.uiTag = uiTag;
        this.amount = 0;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CurrencyDataContainer
{
    [SerializeField] private List<CurrencyData> _currencies = new List<CurrencyData>();

    public List<CurrencyData> Currencies => _currencies;

    public CurrencyDataContainer()
    {
        _currencies = new List<CurrencyData>();
    }

    public CurrencyDataContainer(CurrencyData[] currencies)
    {
        _currencies = new List<CurrencyData>(currencies);
    }

    public void Clear()
    {
        _currencies.Clear();
    }
}
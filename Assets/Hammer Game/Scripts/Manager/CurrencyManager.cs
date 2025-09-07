using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.IO;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private const int DefaultCurrencyCount = 3;

    [Header(" Currency Settings ")]
    [SerializeField] private CurrencyData[] _currencies;
    
    [Header(" Conversion Settings ")]
    [SerializeField] private int _swordToCashRate = 10; // Sword 1���� Cash 10��

    [Header(" Events ")]
    public static Action<CurrencyType, int> OnCurrencyChanged;

    private Dictionary<CurrencyType, CurrencyData> _currencyDict;
    private string _dataPath;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _dataPath = Application.dataPath + "/CurrencyData.txt";

        InitializeCurrencies();
        LoadData();
        UpdateAllCurrencyContainers();
    }

    private void InitializeCurrencies()
    {
        // �⺻ ��ȭ ���� (Inspector���� �������� ���� ���)
        if (_currencies == null || _currencies.Length < DefaultCurrencyCount)
        {
            _currencies = new CurrencyData[]
            {
                new CurrencyData(CurrencyType.Cash, "Cash", "Cash", "CoinAmount"),
                new CurrencyData(CurrencyType.Ruby, "Ruby", "Ruby", "RubyAmount"),
                new CurrencyData(CurrencyType.Sword, "Sword", "Sword", "SwordAmount")
            };
        }

        // Dictionary ����
        _currencyDict = new Dictionary<CurrencyType, CurrencyData>();
        foreach (var currency in _currencies)
        {
            _currencyDict[currency.type] = currency;
        }
    }

    #region Currency Management

    public void AddCurrency(CurrencyType currencyType, int amount)
    {
        if (_currencyDict.ContainsKey(currencyType))
        {
            _currencyDict[currencyType].amount += amount;
            UpdateCurrencyContainers(currencyType);
            
            Debug.Log($"{currencyType}: {_currencyDict[currencyType].amount}");
            
            OnCurrencyChanged?.Invoke(currencyType, _currencyDict[currencyType].amount);
            SaveData();
        }
    }

    public void UseCurrency(CurrencyType currencyType, int amount)
    {
        AddCurrency(currencyType, -amount);
    }

    public int GetCurrency(CurrencyType currencyType)
    {
        return _currencyDict.ContainsKey(currencyType) ? _currencyDict[currencyType].amount : 0;
    }

    public bool HasEnoughCurrency(CurrencyType currencyType, int amount)
    {
        return GetCurrency(currencyType) >= amount;
    }

    #endregion

    #region Legacy Coin Methods (���� ȣȯ��)

    public void AddCoins(int coinEarned)
    {
        AddCurrency(CurrencyType.Cash, coinEarned);
    }

    public void UseCoins(int coinUsed)
    {
        UseCurrency(CurrencyType.Cash, coinUsed);
    }

    public int GetCoins()
    {
        return GetCurrency(CurrencyType.Cash);
    }

    #endregion

    #region Conversion System

    public void ConvertSwordToCash(int swordAmount)
    {
        if (!HasEnoughCurrency(CurrencyType.Sword, swordAmount))
        {
            Debug.LogWarning("Not enough swords to convert!");
            return;
        }

        int cashToAdd = swordAmount * _swordToCashRate;
        
        UseCurrency(CurrencyType.Sword, swordAmount);
        AddCurrency(CurrencyType.Cash, cashToAdd);
        
        Debug.Log($"Converted {swordAmount} swords to {cashToAdd} cash");
    }

    public int GetSwordToCashValue(int swordAmount)
    {
        return swordAmount * _swordToCashRate;
    }

    #endregion

    #region UI Updates

    private void UpdateAllCurrencyContainers()
    {
        foreach (var currency in _currencies)
        {
            UpdateCurrencyContainers(currency.type);
        }
    }

    private void UpdateCurrencyContainers(CurrencyType currencyType)
    {
        if (!_currencyDict.ContainsKey(currencyType))
            return;

        var currencyData = _currencyDict[currencyType];
        
        // �±װ� �����ϴ��� Ȯ�� �� �����ϰ� ó��
        if (!IsTagDefined(currencyData.uiTag))
        {
            Debug.LogWarning($"Tag '{currencyData.uiTag}' is not defined. Skipping UI update for {currencyType}");
            return;
        }

        GameObject[] containers = GameObject.FindGameObjectsWithTag(currencyData.uiTag);

        foreach (GameObject container in containers)
        {
            var textComponent = container.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = currencyData.amount.ToString();
            }
        }
    }

    private bool IsTagDefined(string tag)
    {
        try
        {
            // Unity�� ���� �±״� �׻� ����
            if (tag == "Untagged" || tag == "Respawn" || tag == "Finish" || tag == "EditorOnly" || 
                tag == "MainCamera" || tag == "Player" || tag == "GameController")
            {
                return true;
            }

            // Ŀ���� �±� üũ - �� �迭�� ��ȯ�ϸ� �±װ� ������
            GameObject.FindGameObjectsWithTag(tag);
            return true;
        }
        catch (UnityException)
        {
            return false;
        }
    }

    #endregion

    #region Data Persistence

    private void LoadData()
    {
        try
        {
            if (File.Exists(_dataPath))
            {
                string data = File.ReadAllText(_dataPath);
                
                if (!string.IsNullOrEmpty(data))
                {
                    CurrencyDataContainer container = JsonUtility.FromJson<CurrencyDataContainer>(data);
                    
                    if (container != null && container.Currencies != null)
                    {
                        // �ε�� �����ͷ� ���� currency ������ ������Ʈ
                        foreach (var loadedCurrency in container.Currencies)
                        {
                            if (_currencyDict.ContainsKey(loadedCurrency.type))
                            {
                                _currencyDict[loadedCurrency.type].amount = loadedCurrency.amount;
                            }
                        }
                        Debug.Log("Currency data loaded successfully");
                    }
                    else
                    {
                        Debug.Log("Failed to load currency data, using default values");
                        CreateEmptyCurrencyFile();
                    }
                }
                else
                {
                    CreateEmptyCurrencyFile();
                }
            }
            else
            {
                // ������ �������� ���� �� �� ������ �����ϰ� �⺻�� ����
                CreateEmptyCurrencyFile();
                Debug.Log("Currency file not found, created new currency data");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading currency data: {e.Message}");
            CreateEmptyCurrencyFile();
        }
    }
    
    private void CreateEmptyCurrencyFile()
    {
        try
        {
            // using ���� ����ؼ� ���� ��Ʈ���� �ڵ����� �ݱ�
            using (var fileStream = File.Create(_dataPath))
            {
                // ������ �����ϰ� �ٷ� �ݱ�
            }
            
            // ���� currency �����͸� ����
            SaveData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating currency file: {e.Message}");
        }
    }

    private void SaveData()
    {
        try
        {
            CurrencyDataContainer container = new CurrencyDataContainer(_currencies);
            string data = JsonUtility.ToJson(container, true);
            File.WriteAllText(_dataPath, data);
            Debug.Log("Currency data saved successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving currency data: {e.Message}");
        }
    }

    #endregion

    #region Debug Methods

    [NaughtyAttributes.Button]
    private void Add500Coins()
    {
        AddCurrency(CurrencyType.Cash, 500);
    }

    [NaughtyAttributes.Button]
    private void Add100Ruby()
    {
        AddCurrency(CurrencyType.Ruby, 100);
    }

    [NaughtyAttributes.Button]
    private void Add50Sword()
    {
        AddCurrency(CurrencyType.Sword, 50);
    }

    [NaughtyAttributes.Button]
    private void ConvertAllSwords()
    {
        int swordAmount = GetCurrency(CurrencyType.Sword);
        if (swordAmount > 0)
        {
            ConvertSwordToCash(swordAmount);
        }
    }

    [NaughtyAttributes.Button]
    private void DebugAllCurrencies()
    {
        foreach (var currency in _currencies)
        {
            Debug.Log($"{currency.displayName}: {currency.amount}");
        }
    }

    [NaughtyAttributes.Button]
    private void CheckAllTags()
    {
        foreach (var currency in _currencies)
        {
            bool tagExists = IsTagDefined(currency.uiTag);
            Debug.Log($"Tag '{currency.uiTag}' exists: {tagExists}");
        }
    }

    [NaughtyAttributes.Button]
    private void ClearCurrencyData()
    {
        foreach (var currency in _currencies)
        {
            currency.amount = 0;
        }
        UpdateAllCurrencyContainers();
        SaveData();
        Debug.Log("Currency data cleared");
    }

    #endregion
}

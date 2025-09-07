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
    [SerializeField] private int _swordToCashRate = 10; // Sword 1개당 Cash 10개

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
        // 기본 재화 설정 (Inspector에서 설정하지 않은 경우)
        if (_currencies == null || _currencies.Length < DefaultCurrencyCount)
        {
            _currencies = new CurrencyData[]
            {
                new CurrencyData(CurrencyType.Cash, "Cash", "Cash", "CoinAmount"),
                new CurrencyData(CurrencyType.Ruby, "Ruby", "Ruby", "RubyAmount"),
                new CurrencyData(CurrencyType.Sword, "Sword", "Sword", "SwordAmount")
            };
        }

        // Dictionary 생성
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

    #region Legacy Coin Methods (하위 호환성)

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
        
        // 태그가 존재하는지 확인 후 안전하게 처리
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
            // Unity의 내장 태그는 항상 존재
            if (tag == "Untagged" || tag == "Respawn" || tag == "Finish" || tag == "EditorOnly" || 
                tag == "MainCamera" || tag == "Player" || tag == "GameController")
            {
                return true;
            }

            // 커스텀 태그 체크 - 빈 배열을 반환하면 태그가 존재함
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
                        // 로드된 데이터로 기존 currency 값들을 업데이트
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
                // 파일이 존재하지 않을 때 빈 파일을 생성하고 기본값 유지
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
            // using 구문 사용해서 파일 스트림을 자동으로 닫기
            using (var fileStream = File.Create(_dataPath))
            {
                // 파일을 생성하고 바로 닫기
            }
            
            // 현재 currency 데이터를 저장
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

using System;
using UnityEngine;
using TMPro;

public abstract class UnlockableBase : MonoBehaviour, IUnlockable
{
    [Header(" Unlockable Settings ")]
    [SerializeField] protected GameObject _unlockedElements;
    [SerializeField] protected GameObject _lockedElements;
    [SerializeField] protected TextMeshPro _priceText;
    [SerializeField] protected int _initialPrice;
    [SerializeField] protected CurrencyType _currencyType = CurrencyType.Cash;

    protected int _currentPrice;
    protected bool _isUnlocked = false;
    
    // 디버깅을 위한 고유 ID
    private static int _instanceCounter = 0;
    private int _instanceId;

    // Properties
    public bool IsUnlocked => _isUnlocked;
    public int CurrentPrice => _currentPrice;
    public int InitialPrice => _initialPrice;
    public CurrencyType CurrencyType => _currencyType;

    // Events
    public event Action OnUnlocked;
    public event Action OnPriceChanged;

    private void Awake()
    {
        _instanceId = ++_instanceCounter;
        Debug.Log($"[UnlockableBase #{_instanceId}] Created: {gameObject.name}");
    }

    public virtual void Initialize(int loadedPrice)
    {
        _currentPrice = loadedPrice;
        UpdatePriceDisplay();

        Debug.Log($"[UnlockableBase #{_instanceId}] Initialized: {gameObject.name}, Price: {_currentPrice}");

        if (_currentPrice <= 0)
            Unlock(false);
    }

    public virtual void TryUnlock()
    {
        Debug.Log($"[UnlockableBase #{_instanceId}] TryUnlock called for {gameObject.name}");
        Debug.Log($"[UnlockableBase #{_instanceId}] Current state - IsUnlocked: {_isUnlocked}, CurrentPrice: {_currentPrice}");
        
        // 이미 언락되었으면 무시
        if (_isUnlocked)
        {
            Debug.Log($"[UnlockableBase #{_instanceId}] Already unlocked, skipping");
            return;
        }
            
        // 재화가 부족하면 무시
        int currentCurrency = CurrencyManager.Instance.GetCurrency(_currencyType);
        Debug.Log($"[UnlockableBase #{_instanceId}] Current {_currencyType}: {currentCurrency}");
        
        if (!CanAfford())
        {
            Debug.Log($"[UnlockableBase #{_instanceId}] Cannot afford, skipping");
            return;
        }

        // 언락 시도
        Debug.Log($"[UnlockableBase #{_instanceId}] BEFORE - Price: {_currentPrice}, {_currencyType}: {currentCurrency}");
        
        _currentPrice--;
        CurrencyManager.Instance.UseCurrency(_currencyType, 1);
        
        int newCurrency = CurrencyManager.Instance.GetCurrency(_currencyType);
        Debug.Log($"[UnlockableBase #{_instanceId}] AFTER - Price: {_currentPrice}, {_currencyType}: {newCurrency}");

        OnPriceChanged?.Invoke();
        UpdatePriceDisplay();

        if (_currentPrice <= 0)
        {
            Debug.Log($"[UnlockableBase #{_instanceId}] Fully unlocked!");
            Unlock();
        }
    }

    public virtual void Relock()
    {
        _currentPrice = _initialPrice;
        UpdatePriceDisplay();
        Lock();
    }

    protected virtual bool CanAfford()
    {
        bool canAfford = CurrencyManager.Instance.HasEnoughCurrency(_currencyType, 1);
        Debug.Log($"[UnlockableBase #{_instanceId}] CanAfford: {canAfford}");
        return canAfford;
    }

    protected virtual void Unlock(bool triggerEvent = true)
    {
        if (_unlockedElements != null)
            _unlockedElements.SetActive(true);

        if (_lockedElements != null)
            _lockedElements.SetActive(false);

        _isUnlocked = true;

        if (triggerEvent)
        {
            OnUnlocked?.Invoke();
            OnUnlockComplete();
        }
        
        Debug.Log($"[UnlockableBase #{_instanceId}] {gameObject.name} has been unlocked!");
    }

    protected virtual void Lock()
    {
        if (_unlockedElements != null)
            _unlockedElements.SetActive(false);

        if (_lockedElements != null)
            _lockedElements.SetActive(true);

        _isUnlocked = false;

        OnLockComplete();
    }

    protected virtual void UpdatePriceDisplay()
    {
        if (_priceText != null)
            _priceText.text = _currentPrice.ToString();
    }

    protected abstract void OnUnlockComplete();
    protected abstract void OnLockComplete();

    public virtual void DisplayLockedElements()
    {
        if (_lockedElements != null)
            _lockedElements.SetActive(true);
    }
}
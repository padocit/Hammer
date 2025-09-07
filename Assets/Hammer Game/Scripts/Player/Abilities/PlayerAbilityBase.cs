using UnityEngine;
using System.Collections.Generic;

public abstract class PlayerAbilityBase<T> : MonoBehaviour, IPlayerAbility, ITargetHandler<T> 
    where T : Component
{
    [Header("Ability Settings")]
    [SerializeField] protected PlayerToolSelector.Tool _requiredTool;
    [SerializeField] protected string _targetTag;
    
    protected PlayerAnimator _playerAnimator;
    protected PlayerToolSelector _playerToolSelector;
    protected T _currentTarget;
    protected bool _isActive;
    
    // ������Ʈ ĳ������ ���� ���� (WeakReference�� �޸� ����ȭ)
    private readonly Dictionary<GameObject, T> _componentCache = new Dictionary<GameObject, T>();
    private const int MAX_CACHE_SIZE = 50; // ĳ�� ũ�� ����
    
    public bool IsActive => _isActive;
    
    protected virtual void Awake()
    {
        CacheComponents();
        SubscribeToEvents();
    }
    
    protected virtual void OnDestroy()
    {
        UnsubscribeFromEvents();
        _componentCache.Clear();
    }
    
    private void CacheComponents()
    {
        _playerAnimator = GetComponent<PlayerAnimator>();
        _playerToolSelector = GetComponent<PlayerToolSelector>();
    }
    
    protected virtual void SubscribeToEvents()
    {
        if (_playerToolSelector != null)
        {
            _playerToolSelector.OnToolSelected += OnToolSelectedCallback;
        }
    }
    
    protected virtual void UnsubscribeFromEvents()
    {
        if (_playerToolSelector != null)
        {
            _playerToolSelector.OnToolSelected -= OnToolSelectedCallback;
        }
    }
    
    protected virtual void OnToolSelectedCallback(PlayerToolSelector.Tool selectedTool)
    {
        if (selectedTool != _requiredTool && _isActive)
        {
            StopInteraction();
        }
    }
    
    // ������Ʈ ĳ������ GetComponent ȣ�� �ּ�ȭ (�޸� ���� ����)
    protected T GetCachedComponent(GameObject obj)
    {
        if (!_componentCache.TryGetValue(obj, out T component))
        {
            component = obj.GetComponent<T>();
            if (component != null)
            {
                // ĳ�� ũ�� ����
                if (_componentCache.Count >= MAX_CACHE_SIZE)
                {
                    // ������ ĳ�� ��Ʈ�� ���� (������ LRU ����)
                    var oldestKey = GetOldestCacheKey();
                    if (oldestKey != null)
                    {
                        _componentCache.Remove(oldestKey);
                    }
                }
                
                _componentCache[obj] = component;
            }
        }
        return component;
    }
    
    private GameObject GetOldestCacheKey()
    {
        // ������ ����: ù ��° Ű�� ��ȯ
        foreach (var key in _componentCache.Keys)
        {
            return key;
        }
        return null;
    }
    
    public virtual void Activate()
    {
        _isActive = true;
        if (_playerToolSelector != null)
        {
            _playerToolSelector.SelectTool((int)_requiredTool);
        }
    }
    
    // ��ȯ ȣ�� ����: StopInteraction() ȣ�� ����
    public virtual void Deactivate()
    {
        _isActive = false;
        
        // ���� �����ϰ� StopInteraction�� ȣ������ ����
        if (_playerToolSelector != null)
        {
            _playerToolSelector.SelectTool(0); // Tool.None�� �ش�
        }
    }
    
    // ������ ������ ���� ���� �޼���
    protected virtual void DeactivateInternal()
    {
        _isActive = false;
        _currentTarget = null;
        
        if (_playerToolSelector != null)
        {
            _playerToolSelector.SelectTool(0);
        }
    }
    
    public void HandleTargetEntered<TTarget>(TTarget target) where TTarget : Component
    {
        if (target is T typedTarget && CanInteract(typedTarget))
        {
            StartInteraction(typedTarget);
        }
    }
    
    public void HandleTargetExited<TTarget>(TTarget target) where TTarget : Component
    {
        if (target is T typedTarget && typedTarget == _currentTarget)
        {
            StopInteraction();
        }
    }
    
    // Ʈ���� �̺�Ʈ ���� ó��
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_targetTag))
        {
            var target = GetCachedComponent(other.gameObject);
            if (target != null)
            {
                HandleTargetEntered(target);
            }
        }
    }
    
    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(_targetTag))
        {
            var target = GetCachedComponent(other.gameObject);
            if (target != null && CanInteract(target))
            {
                StartInteraction(target);
            }
        }
    }
    
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_targetTag))
        {
            var target = GetCachedComponent(other.gameObject);
            if (target != null)
            {
                HandleTargetExited(target);
            }
        }
    }
    
    // �߻� �޼���� - ��ü���� ������ ���� Ŭ��������
    public abstract bool CanInteract(T target);
    public abstract void StartInteraction(T target);
    public abstract void StopInteraction();
}
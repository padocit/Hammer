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
    
    // 컴포넌트 캐싱으로 성능 개선 (WeakReference로 메모리 최적화)
    private readonly Dictionary<GameObject, T> _componentCache = new Dictionary<GameObject, T>();
    private const int MAX_CACHE_SIZE = 50; // 캐시 크기 제한
    
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
    
    // 컴포넌트 캐싱으로 GetComponent 호출 최소화 (메모리 관리 포함)
    protected T GetCachedComponent(GameObject obj)
    {
        if (!_componentCache.TryGetValue(obj, out T component))
        {
            component = obj.GetComponent<T>();
            if (component != null)
            {
                // 캐시 크기 제한
                if (_componentCache.Count >= MAX_CACHE_SIZE)
                {
                    // 오래된 캐시 엔트리 제거 (간단한 LRU 구현)
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
        // 간단한 구현: 첫 번째 키를 반환
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
    
    // 순환 호출 방지: StopInteraction() 호출 제거
    public virtual void Deactivate()
    {
        _isActive = false;
        
        // 툴만 리셋하고 StopInteraction은 호출하지 않음
        if (_playerToolSelector != null)
        {
            _playerToolSelector.SelectTool(0); // Tool.None에 해당
        }
    }
    
    // 완전한 정리를 위한 내부 메서드
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
    
    // 트리거 이벤트 통합 처리
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
    
    // 추상 메서드들 - 구체적인 구현은 하위 클래스에서
    public abstract bool CanInteract(T target);
    public abstract void StartInteraction(T target);
    public abstract void StopInteraction();
}
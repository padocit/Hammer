using UnityEngine;

[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerToolSelector))]
public class PlayerResourceAbility : PlayerAbilityBase<ResourceNode>
{
    [Header("Resource Settings")]
    [SerializeField] private Transform _resourceSphere;
    
    private bool _canGatherResource;
    private bool _isGathering;
    
    protected override void Awake()
    {
        _requiredTool = PlayerToolSelector.Tool.Resource;
        _targetTag = "ResourceNode";
        base.Awake();
    }
    
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        ResourceNode.OnFullyHarvested += OnResourceNodeFullyHarvested;
    }
    
    protected override void UnsubscribeFromEvents()
    {
        base.UnsubscribeFromEvents();
        ResourceNode.OnFullyHarvested -= OnResourceNodeFullyHarvested;
    }
    
    private void OnResourceNodeFullyHarvested(ResourceNode resourceNode)
    {
        if (resourceNode == _currentTarget)
        {
            StopInteraction();
        }
    }
    
    // 자동 툴 선택을 위해 툴 체크 제거
    public override bool CanInteract(ResourceNode target)
    {
        return target != null && target.CanGather();
    }
    
    public override void StartInteraction(ResourceNode target)
    {
        if (_currentTarget == target && _isActive) 
            return;
            
        // 이미 다른 리소스를 채집 중이라면 중단
        if (_isGathering)
            return;
            
        _currentTarget = target;
        Activate(); // 자동 툴 선택
        
        // 리소스 타입에 따른 애니메이션 재생
        PlayResourceAnimation();
        
        Debug.Log($"Started gathering from: {target.name} ({target.GetResourceNodeType()})");
    }
    
    public override void StopInteraction()
    {
        if (_currentTarget != null)
        {
            StopResourceAnimation();
            Debug.Log($"Stopped gathering from: {_currentTarget.name}");
        }
        
        _isGathering = false;
        
        // DeactivateInternal 사용으로 순환 호출 방지
        DeactivateInternal();
    }
    
    private void PlayResourceAnimation()
    {
        if (_currentTarget == null || _playerAnimator == null)
            return;
            
        switch (_currentTarget.GetResourceNodeType())
        {
            case ResourceNodeType.Tree:
                _playerAnimator.PlayTreeAnimation();
                break;
            case ResourceNodeType.Rock:
                _playerAnimator.PlayRockAnimation();
                break;
        }
    }
    
    private void StopResourceAnimation()
    {
        if (_currentTarget == null || _playerAnimator == null)
            return;
            
        switch (_currentTarget.GetResourceNodeType())
        {
            case ResourceNodeType.Tree:
                _playerAnimator.StopTreeAnimation();
                break;
            case ResourceNodeType.Rock:
                _playerAnimator.StopRockAnimation();
                break;
        }
    }
    
    protected override void OnToolSelectedCallback(PlayerToolSelector.Tool selectedTool)
    {
        // 외부에서 다른 툴이 선택되면 중단
        if (selectedTool != _requiredTool && _isActive)
        {
            StopInteraction();
        }
    }
    
    // 애니메이션 이벤트 콜백
    public void OnResourceGatheringStarted()
    {
        _canGatherResource = true;
        
        // 이미 타겟이 설정되어 있고 채집 가능하다면 시작
        if (_currentTarget != null && _currentTarget.CanGather() && !_isGathering)
        {
            _isGathering = true;
            _currentTarget.Gather();
        }
    }
    
    public void OnResourceGatheringStopped()
    {
        _canGatherResource = false;
        _isGathering = false;
    }
    
    public ResourceType GetCurrentResourceType()
    {
        return _currentTarget?.GetResourceType() ?? ResourceType.Wood;
    }
}

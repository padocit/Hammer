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
    
    // �ڵ� �� ������ ���� �� üũ ����
    public override bool CanInteract(ResourceNode target)
    {
        return target != null && target.CanGather();
    }
    
    public override void StartInteraction(ResourceNode target)
    {
        if (_currentTarget == target && _isActive) 
            return;
            
        // �̹� �ٸ� ���ҽ��� ä�� ���̶�� �ߴ�
        if (_isGathering)
            return;
            
        _currentTarget = target;
        Activate(); // �ڵ� �� ����
        
        // ���ҽ� Ÿ�Կ� ���� �ִϸ��̼� ���
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
        
        // DeactivateInternal ������� ��ȯ ȣ�� ����
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
        // �ܺο��� �ٸ� ���� ���õǸ� �ߴ�
        if (selectedTool != _requiredTool && _isActive)
        {
            StopInteraction();
        }
    }
    
    // �ִϸ��̼� �̺�Ʈ �ݹ�
    public void OnResourceGatheringStarted()
    {
        _canGatherResource = true;
        
        // �̹� Ÿ���� �����Ǿ� �ְ� ä�� �����ϴٸ� ����
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

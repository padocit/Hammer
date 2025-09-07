using UnityEngine;

[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerToolSelector))]
public class PlayerHarvestAbility : PlayerAbilityBase<CropField>
{
    [Header("Harvest Settings")]
    [SerializeField] private Transform _harvestSphere;
    private bool _canHarvest;
    
    protected override void Awake()
    {
        _requiredTool = PlayerToolSelector.Tool.Harvest;
        _targetTag = "CropField";
        base.Awake();
    }
    
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        CropField.OnFullyHarvested += OnCropFieldFullyHarvested;
    }
    
    protected override void UnsubscribeFromEvents()
    {
        base.UnsubscribeFromEvents();
        CropField.OnFullyHarvested -= OnCropFieldFullyHarvested;
    }
    
    private void OnCropFieldFullyHarvested(CropField cropField)
    {
        if (cropField == _currentTarget)
        {
            StopInteraction();
        }
    }
    
    public override bool CanInteract(CropField target)
    {
        return target != null && target.IsWatered() && _playerToolSelector.CanHarvest();
    }
    
    public override void StartInteraction(CropField target)
    {
        if (_currentTarget == target) return;
        
        _currentTarget = target;
        Activate();
        
        _playerAnimator.PlayHarvestAnimation();
        
        if (_canHarvest)
        {
            _currentTarget.Harvest(_harvestSphere);
        }
    }
    
    public override void StopInteraction()
    {
        if (_currentTarget != null)
        {
            _playerAnimator.StopHarvestAnimation();
            _currentTarget = null;
        }
        Deactivate();
    }
    
    // 애니메이션 이벤트 콜백
    public void OnHarvestingStarted() => _canHarvest = true;
    public void OnHarvestingStopped() => _canHarvest = false;
}